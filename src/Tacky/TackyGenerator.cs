using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public class TackyGenerator(RuntimeState opts)
{
    public RuntimeState Options = opts;

    internal List<TacFunction> functions = [];
    internal List<ITackyInstr> instructions = [];

    internal TacVar? LastTmpVar;
    internal int TmpVarCounter = 0;
    internal TacVar ReserveTmpVar()
    {
        if (functions.Count > 0)
        {
            var tv = new TacVar($"tmp.{TmpVarCounter++}");
            LastTmpVar = tv;
            functions[^1].Locals.Add(tv);
            return tv;
        }
        else
        {
            throw new TackyGenError("cannot allocate tmp var with no function declared");
        }
    }

    internal TacVar? GetLastTmpVar() => LastTmpVar;
    internal TacVar GetLastTmpVarOrFail() => GetLastTmpVar() ?? throw new TackyGenError("need to know last temp var in UnaryOp, but none available");

    internal int TmpLabelCounter = 0;
    internal string ReserveTmpLabel(string prefix = "_l") => $"{prefix}{TmpLabelCounter++}";

    private void Emit(ITackyInstr instr)
    {
        instructions.Add(instr);
        // Console.Error.WriteLine($"> {instr}");
    }

    public bool Execute()
    {
        EmitTacky(Options.Ast);
        Options.Tacky = new TacProgram(functions);

        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("TAC IR:");
            Console.Error.WriteLine("=======");
        }

        if (Options.Verbose || Options.OnlyThroughTacky)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;
            foreach (var f in Options.Tacky.Functions)
            {
                foreach (var i in f.Instructions)
                {
                    stream.WriteLine(i switch
                    {
                        TacConstant or TacVar => i.ToString(),
                        TacLabel l => $"\n{l.Identifier}:",
                        _ => $"    {i}"
                    });
                }
                stream.WriteLine();
            }
        }

        return true;
    }

    internal TacVal EmitTacky(IAstNode node)
    {
        switch (node)
        {
            case Ast.Program p:
                foreach (var s in p.Statements)
                {
                    EmitTacky(s);
                }
                return DUMMY;

            case Function f:
                instructions = [];
                functions.Add(new TacFunction(f.Name, instructions));
                TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually
                foreach (var s in f.Body)
                {
                    EmitTacky(s);
                }
                return DUMMY;

            case Return r:
                var result = EmitTacky(r.Expr);
                Emit(new TacReturn(result));
                return result;

            case Constant c:
                return new TacConstant(c.Int);

            case UnaryOp u:
                var src = EmitTacky(u.Expr);
                var dst = ReserveTmpVar();
                Emit(new TacUnary(u.Op, src, dst));
                return dst;

            case BinaryOp b when b.Op == "&&":
                return EmitLogicalAnd(b);

            case BinaryOp b when b.Op == "||":
                return EmitLogicalOr(b);

            case BinaryOp b:
                var src1 = EmitTacky(b.LExpr);
                var src2 = EmitTacky(b.RExpr);
                dst = ReserveTmpVar();
                Emit(new TacBinary(b.Op, src1, src2, dst));
                return dst;

            default:
                throw new NotImplementedException($"{GetType().Name}.{nameof(EmitTacky)} can't handle {node.GetType().Name} yet");
        }
    }

    internal TacVal TacConstantOrExpression(IAstNode n)
    {
        if (n is Constant c)
        {
            var tacConstant = new TacConstant(c.Int);
            return tacConstant;
        }
        else
        {
            EmitTacky(n);
            var pseudoVar = GetLastTmpVarOrFail();
            return pseudoVar;
        }
    }

    internal TacVar EmitLogicalAnd(BinaryOp b)
    {
        var falseLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var v1 = ReserveTmpVar();
        var result = ReserveTmpVar();

        // evaluate lexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var lexpr = EmitTacky(b.LExpr);
        Emit(new TacCopy(lexpr, v1));
        Emit(new TacJumpIfZero(v1, falseLabel));

        // evaluate rexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var rexpr = EmitTacky(b.RExpr);
        Emit(new TacCopy(rexpr, v1));
        Emit(new TacJumpIfZero(v1, falseLabel));

        // set result to 1 and jump to `endLabel`
        Emit(new TacCopy(new TacConstant(1), result));
        Emit(new TacJump(endLabel));

        // set result to 0 and fall through to end
        Emit(new TacLabel(falseLabel));
        Emit(new TacCopy(new TacConstant(0), result));
        Emit(new TacLabel(endLabel));

        return result;
    }

    internal TacVar EmitLogicalOr(BinaryOp b)
    {
        var trueLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var v1 = ReserveTmpVar();
        var result = ReserveTmpVar();

        // Evaluate lexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        var lexpr = EmitTacky(b.LExpr);
        Emit(new TacCopy(lexpr, v1));
        Emit(new TacJumpIfNotZero(v1, trueLabel));

        // Evaluate rexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        var rexpr = EmitTacky(b.RExpr);
        Emit(new TacCopy(rexpr, v1));
        Emit(new TacJumpIfNotZero(v1, trueLabel));

        // set result to 0 and jump to `endLabel`
        Emit(new TacCopy(new TacConstant(0), result));
        Emit(new TacJump(endLabel));

        // set result to 1 and fall through to end
        Emit(new TacLabel(trueLabel));
        Emit(new TacCopy(new TacConstant(1), result));
        Emit(new TacLabel(endLabel));

        return result;
    }

    internal static TacVar DUMMY = new("DUMMY");
}