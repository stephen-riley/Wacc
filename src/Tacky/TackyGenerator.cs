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
                    stream.WriteLine(i);
                }
                stream.WriteLine();
            }
        }

        return true;
    }

    internal void EmitTacky(IAstNode node)
    {
        switch (node)
        {
            case Ast.Program p:
                foreach (var s in p.Statements)
                {
                    EmitTacky(s);
                }
                break;

            case Function f:
                instructions = [];
                functions.Add(new TacFunction(f.Name, instructions));
                TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually
                EmitTacky(f.Body);
                break;

            case Return r:
                var retExpr = TacConstantOrExpression(r.Expr);
                Emit(new TacReturn(retExpr));
                break;

            case Constant c:
                Emit(new TacConstant(c.Int));
                break;

            case UnaryOp u:
                var src = TacConstantOrExpression(u.Expr);
                var dst = ReserveTmpVar();
                Emit(new TacUnary(u.Op, src, dst));
                break;

            case BinaryOp b when b.Op == "&&":
                EmitLogicalAnd(b);
                break;

            case BinaryOp b when b.Op == "||":
                EmitLogicalOr(b);
                break;

            case BinaryOp b:
                var src1 = TacConstantOrExpression(b.LExpr);
                var src2 = TacConstantOrExpression(b.RExpr);
                dst = ReserveTmpVar();
                Emit(new TacBinary(b.Op, src1, src2, dst));
                break;

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

    internal void EmitLogicalAnd(BinaryOp b)
    {
        var falseLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var v1 = ReserveTmpVar();
        // var v2 = ReserveTmpVar();
        var result = ReserveTmpVar();

        // evaluate lexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var lexpr = TacConstantOrExpression(b.LExpr);
        Emit(new TacCopy(lexpr, v1));
        Emit(new TacJumpIfZero(v1, falseLabel));

        // evaluate rexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var rexpr = TacConstantOrExpression(b.RExpr);
        Emit(new TacCopy(rexpr, v1));
        Emit(new TacJumpIfZero(v1, falseLabel));

        // set result to 1 and jump to `endLabel`
        Emit(new TacCopy(new TacConstant(1), result));
        Emit(new TacJump(endLabel));

        // set result to 0 and fall through to end
        Emit(new TacLabel(falseLabel));
        Emit(new TacCopy(new TacConstant(0), result));
        Emit(new TacLabel(endLabel));
        // Emit(new TacCopy)
    }

    internal void EmitLogicalOr(BinaryOp b)
    {
        var trueLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var v1 = ReserveTmpVar();
        var v2 = ReserveTmpVar();
        var result = ReserveTmpVar();

        // Evaluate lexpr.  If true, jump to `trueLabel`; therwise, fall through.
        TacConstantOrExpression(b.LExpr);
        Emit(new TacCopy(GetLastTmpVarOrFail(), v1));
        Emit(new TacJumpIfNotZero(v1, trueLabel));

        // Evaluate rexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        TacConstantOrExpression(b.RExpr);
        Emit(new TacCopy(GetLastTmpVarOrFail(), v2));
        Emit(new TacJumpIfNotZero(v2, trueLabel));

        // set result to 0 and jump to `endLabel`
        Emit(new TacCopy(new TacConstant(0), result));
        Emit(new TacJump(endLabel));

        // set result to 1 and fall through to end
        Emit(new TacLabel(trueLabel));
        Emit(new TacCopy(new TacConstant(1), result));
        Emit(new TacLabel(endLabel));
    }
}