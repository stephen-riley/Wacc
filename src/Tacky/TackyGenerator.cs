using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public class TackyGenerator(RuntimeState opts)
{
    internal static TacVar DUMMY = new("DUMMY");

    public RuntimeState Options = opts;

    internal List<TacFunction> functions = [];
    internal List<ITackyInstr> instructions = [];

    internal HashSet<string> ReservedLabels = ["_main"];

    internal Stack<string> BreakLabelStack = [];
    internal Stack<string> ContinueLabelStack = [];

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

    internal TacVar RegisterVar(TacVar tv)
    {
        functions[^1].Locals.Add(tv);
        return tv;
    }

    internal string GetCleanLabelName(string id) => ReservedLabels.Contains(id) ? $"{id}_X" : id;

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

        if (!Options.Silent)
        {
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
        }

        return true;
    }

    internal TacVal EmitTacky(IAstNode node)
    {
        switch (node)
        {
            case CompUnit p:
                foreach (var s in p.Functions)
                {
                    EmitTacky(s);
                }
                return DUMMY;

            case Block b:
                foreach (var bi in b.BlockItems)
                {
                    EmitTacky(bi);
                }
                return DUMMY;

            case Function f:
                instructions = [];
                functions.Add(new TacFunction(f.Name, instructions));
                TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually
                EmitTacky(f.Body);
                Emit(new TacReturn(new TacConstant(0)));
                return DUMMY;

            case Var v:
                return RegisterVar(new TacVar(v.Name));

            case Declaration d when d.Expr is null:
                return RegisterVar(new TacVar(d.Identifier.Name));

            case Declaration d when d.Expr is not null:
                var declResult = EmitTacky(d.Expr);
                Emit(new TacCopy(declResult, RegisterVar(new TacVar(d.Identifier.Name))));
                return declResult;

            case Assignment a when a.LExpr is Var v:
                var rhsResult = EmitTacky(a.RExpr);
                Emit(new TacCopy(rhsResult, new TacVar(v.Name)));
                return new TacVar(v.Name);

            case OpAssignment oa when oa.LExpr is Var v:
                rhsResult = EmitTacky(oa.RExpr);
                return new TacVar(v.Name);

            case Return r:
                var retResult = EmitTacky(r.Expr);
                Emit(new TacReturn(retResult));
                return retResult;

            case Goto g:
                Emit(new TacJump(GetCleanLabelName(g.Label?.Name ?? throw new InvalidOperationException("BlockItem.LabelName cannot be null here"))));
                return DUMMY;

            case Label l:
                Emit(new TacLabel(GetCleanLabelName(l.Name)));
                return DUMMY;

            case Constant c:
                return new TacConstant(c.Int);

            case Expression e:
                return EmitTacky(e.SubExpr);

            case UnaryOp u:
                var src = EmitTacky(u.Expr);
                var dst = ReserveTmpVar();
                Emit(new TacUnary(u.Op.TokenType, src, dst));
                return dst;

            case BinaryOp b when b.Op == TokenType.LogicalAnd:
                return EmitLogicalAnd(b);

            case BinaryOp b when b.Op == TokenType.LogicalOr:
                return EmitLogicalOr(b);

            case BinaryOp b:
                var src1 = EmitTacky(b.LExpr);
                var src2 = EmitTacky(b.RExpr);
                dst = ReserveTmpVar();
                Emit(new TacBinary(b.Op, src1, src2, dst));
                return dst;

            case PrefixOp pe:
                var peOp = pe.Op == TokenType.Increment ? TokenType.Plus : TokenType.Minus;
                dst = ReserveTmpVar();
                src1 = EmitTacky(pe.LValExpr);
                Emit(new TacBinary(peOp, src1, new TacConstant(1), (TacVar)src1));
                Emit(new TacCopy(src1, dst));
                return dst;

            case PostfixOp po:
                var poOp = po.Op == TokenType.Increment ? TokenType.Plus : TokenType.Minus;
                dst = ReserveTmpVar();
                src1 = EmitTacky(po.LValExpr);
                Emit(new TacCopy(src1, dst));
                Emit(new TacBinary(poOp, src1, new TacConstant(1), (TacVar)src1));
                return dst;

            case IfElse ie when ie.ElseBlock is null:
                var endLabel = ReserveTmpLabel();
                var cond = EmitTacky(ie.CondExpr);
                Emit(new TacJumpIfZero(cond, endLabel));
                EmitTacky(ie.ThenBlock);
                Emit(new TacLabel(endLabel));
                return DUMMY;

            case IfElse ie when ie.ElseBlock is not null:
                var elseLabel = ReserveTmpLabel();
                endLabel = ReserveTmpLabel();
                cond = EmitTacky(ie.CondExpr);
                Emit(new TacJumpIfZero(cond, elseLabel));
                EmitTacky(ie.ThenBlock);
                Emit(new TacJump(endLabel));
                Emit(new TacLabel(elseLabel));
                EmitTacky(ie.ElseBlock);
                Emit(new TacLabel(endLabel));
                return DUMMY;

            case Ternary t:
                var altLabel = ReserveTmpLabel();
                endLabel = ReserveTmpLabel();
                var result = ReserveTmpVar();
                cond = EmitTacky(t.CondExpr);
                Emit(new TacJumpIfZero(cond, altLabel));
                var middle = EmitTacky(t.Middle);
                Emit(new TacCopy(middle, result));
                Emit(new TacJump(endLabel));
                Emit(new TacLabel(altLabel));
                var right = EmitTacky(t.Right);
                Emit(new TacCopy(right, result));
                Emit(new TacLabel(endLabel));
                return result;

            case ForLoop fl:
                var startLabel = ReserveTmpLabel("_for");
                var contLabel = ReserveTmpLabel("_for");
                endLabel = ReserveTmpLabel("_for");
                BreakLabelStack.Push(endLabel);
                ContinueLabelStack.Push(contLabel);

                endLabel = ReserveTmpLabel("_for");
                EmitTacky(fl.InitStat);
                Emit(new TacLabel(startLabel));
                cond = EmitTacky(fl.CondExpr);
                Emit(new TacJumpIfZero(cond, endLabel));
                EmitTacky(fl.BodyBlock);
                Emit(new TacLabel(contLabel));
                EmitTacky(fl.PostStat);
                Emit(new TacJump(startLabel));
                Emit(new TacLabel(endLabel));

                BreakLabelStack.Pop();
                ContinueLabelStack.Pop();
                return DUMMY;

            case DoLoop dl:
                startLabel = ReserveTmpLabel("_do");
                var condLabel = ReserveTmpLabel("_do");
                endLabel = ReserveTmpLabel("_do");
                BreakLabelStack.Push(endLabel);
                ContinueLabelStack.Push(condLabel);

                Emit(new TacLabel(startLabel));
                EmitTacky(dl.BodyBlock);
                Emit(new TacLabel(condLabel));
                var condResult = EmitTacky(dl.CondExpr);
                Emit(new TacJumpIfNotZero(condResult, startLabel));
                Emit(new TacLabel(endLabel));

                BreakLabelStack.Pop();
                ContinueLabelStack.Pop();
                return DUMMY;

            case WhileLoop wl:
                startLabel = ReserveTmpLabel("_wh");
                endLabel = ReserveTmpLabel("_wh");
                BreakLabelStack.Push(endLabel);
                ContinueLabelStack.Push(startLabel);

                Emit(new TacLabel(startLabel));
                condResult = EmitTacky(wl.CondExpr);
                Emit(new TacJumpIfZero(condResult, endLabel));
                EmitTacky(wl.BodyBlock);
                Emit(new TacJump(startLabel));
                Emit(new TacLabel(endLabel));

                BreakLabelStack.Pop();
                ContinueLabelStack.Pop();
                return DUMMY;

            case Break:
                if (!BreakLabelStack.TryPeek(out var breakLabel))
                {
                    throw new TackyGenError("no break label in this scope");
                }
                Emit(new TacJump(breakLabel));
                return DUMMY;

            case Continue:
                if (!BreakLabelStack.TryPeek(out contLabel))
                {
                    throw new TackyGenError("no break label in this scope");
                }
                Emit(new TacJump(contLabel));
                return DUMMY;

            case Default d:
                EmitTacky(d.DefaultBlock);
                return DUMMY;

            case LabeledBlock ls:
                Emit(new TacLabel(GetCleanLabelName(ls.Label.Name)));
                return EmitTacky(ls.Stat);

            case NullStatement:
                return DUMMY;

            case Switch sw:
                return EmitSwitch(sw);

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

    internal TacVar EmitSwitch(Switch sw)
    {
        var endLabel = ReserveTmpLabel("_sc");
        BreakLabelStack.Push(endLabel);

        var condVar = EmitTacky(sw.CondExpr);
        var block = sw.CaseBlock as Block ?? throw new TackyGenError($"body of switch-case is not a Block");

        var nextLabel = ReserveTmpLabel("_sc");
        foreach (var stat in block.BlockItems)
        {
            if (stat is Case c)
            {
                var caseVal = EmitTacky(c.CaseCondExpr);
                var dest = ReserveTmpVar();
                Emit(new TacBinary(TokenType.EqualTo, condVar, caseVal, dest));
                Emit(new TacJumpIfNotZero(dest, nextLabel));
                EmitTacky(c.CaseBlock);
                Emit(new TacLabel(nextLabel));
                nextLabel = ReserveTmpLabel("_sc");
            }
            else if (stat is Default d)
            {
                EmitTacky(d.DefaultBlock);
            }
        }
        Emit(new TacLabel(endLabel));

        BreakLabelStack.Pop();
        return DUMMY;
    }
}