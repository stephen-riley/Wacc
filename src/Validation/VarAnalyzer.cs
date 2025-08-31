using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Validation;

public class VarAnalyzer : BaseAstRewriter
{
    internal Dictionary<string, int> UniqueVarCounters = [];

    public override CompUnit Validate(CompUnit program)
    {
        var variableMap = new VarMap();
        var newAst = ResolveStatement(program, variableMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override IAstNode OnBlockStat(Block stat, VarMap variableMap)
        => base.OnBlockStat(stat, new VarMap(variableMap));

    public override IAstNode OnDeclarationStat(Declaration stat, VarMap variableMap)
    {
        var (declType, ident, init) = stat;

        if (variableMap.TryGetValue(ident.Name, out var value, out var inCurScope) && inCurScope)
        {
            throw new ValidationError($"duplicate variable declaration for {ident}");
        }

        var uniqueName = GenUniqueVarName(ident.Name);
        variableMap[ident.Name] = uniqueName;

        if (init is not null)
        {
            init = ResolveExpr(init, variableMap);
        }

        return new Declaration(declType, new Var(uniqueName), init);
    }

    public override IAstNode OnDoLoopStat(DoLoop stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        return (DoLoop)base.OnDoLoopStat(stat, newMap) with { VariableMap = newMap };
    }

    public override IAstNode OnForLoopStat(ForLoop stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        return (ForLoop)base.OnForLoopStat(stat, newMap) with { VariableMap = newMap };
    }

    public override IAstNode OnFunction(Function stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        return (Function)base.OnFunction(stat, newMap) with { VariableMap = newMap };
    }

    public override IAstNode OnWhileLoopStat(WhileLoop stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        return (WhileLoop)base.OnWhileLoopStat(stat, newMap) with { VariableMap = newMap };
    }

    public override IAstNode OnVarExpr(Var expr, VarMap variableMap)
    {
        if (variableMap.TryGetValue(expr.Name, out var globalName, out _))
        {
            return new Var(globalName);
        }
        else
        {
            throw new ValidationError($"Undeclared variable {expr.Name}");
        }
    }

    internal string GenUniqueVarName(string name)
    {
        if (UniqueVarCounters.TryGetValue(name, out int value))
        {
            UniqueVarCounters[name] = ++value;
        }
        else
        {
            UniqueVarCounters[name] = 0;
        }
        return $"${name}_{UniqueVarCounters[name]}";
    }

    internal IAstNode OldResolveStatement(IAstNode stat, VarMap variableMap)
    {
        return stat switch
        {
            Assignment a => ResolveExpr(a, variableMap),
            BinaryOp bo => ResolveExpr(bo, variableMap),
            Block b => Ext.Do(() =>
            {
                var newMap = new VarMap(variableMap);
                var blockItems = new List<IAstNode>();
                foreach (var item in b.BlockItems)
                {
                    blockItems.Add(ResolveStatement(item, newMap));
                }
                var newBlock = new Block([.. blockItems]) with { VariableMap = newMap };
                return newBlock;
            }),
            Break br => br,
            Continue c => c,
            Declaration d => Ext.Do(() =>
            {
                var (declType, ident, init) = d;

                if (variableMap.TryGetValue(ident.Name, out var value, out var inCurScope) && inCurScope)
                {
                    throw new ValidationError($"duplicate variable declaration for {ident}");
                }

                var uniqueName = GenUniqueVarName(ident.Name);
                variableMap[ident.Name] = uniqueName;

                if (init is not null)
                {
                    init = ResolveExpr(init, variableMap);
                }

                return new Declaration(declType, new Var(uniqueName), init);
            }),
            DoLoop dl => Ext.Do(() =>
            {
                var newMap = new VarMap(variableMap);
                var cond = dl.CondExpr is NullStatement ? dl.CondExpr : ResolveExpr(dl.CondExpr, newMap);
                var body = ResolveStatement(dl.BodyBlock, newMap);
                var newDo = new DoLoop(body, cond, dl.Label);
                return newDo with { VariableMap = newMap };
            }),
            Expression e => new Expression(ResolveExpr(e.SubExpr, variableMap)),
            ForLoop fl => Ext.Do(() =>
            {
                var newMap = new VarMap(variableMap);
                var init = ResolveStatement(fl.InitStat, newMap);
                var cond = fl.CondExpr is NullStatement ? fl.CondExpr : ResolveExpr(fl.CondExpr, newMap);
                var post = fl.CondExpr is NullStatement ? fl.UpdateStat : ResolveExpr(fl.UpdateStat, newMap);
                var body = ResolveStatement(fl.BodyBlock, newMap);
                var newFor = new ForLoop(init, cond, post, body, fl.Label);
                return newFor with { VariableMap = newMap };
            }),
            IfElse ie => new IfElse(
                ResolveExpr(ie.CondExpr, variableMap),
                ResolveStatement(ie.ThenBlock, variableMap),
                ie.ElseBlock is not null ? ResolveStatement(ie.ElseBlock, variableMap) : null
            ),
            LabeledStatement ls => new LabeledStatement(
                ls.Label,
                ResolveStatement(ls.Stat, variableMap)
            ),
            NullStatement => stat,
            PostfixOp po => ResolveExpr(po, variableMap),
            PrefixOp pr => ResolveExpr(pr, variableMap),
            Return r => new Return(ResolveExpr(r.Expr, variableMap)),
            Ternary t => new Ternary(
                ResolveExpr(t.CondExpr, variableMap),
                ResolveExpr(t.Middle, variableMap),
                ResolveExpr(t.Right, variableMap)
            ),
            WhileLoop wl => Ext.Do(() =>
            {
                var newMap = new VarMap(variableMap);
                var cond = wl.CondExpr is NullStatement ? wl.CondExpr : ResolveExpr(wl.CondExpr, newMap);
                var body = ResolveStatement(wl.BodyBlock, newMap);
                var newWhile = new WhileLoop(cond, body, wl.Label);
                return newWhile with { VariableMap = newMap };
            }),
            _ => stat
        };
    }

    internal IAstNode OldResolveExpr(IAstNode e, VarMap variableMap)
    {
        switch (e)
        {
            case Assignment a:
                if (a.LExpr is not Var)
                {
                    throw new ValidationError($"Invalid lval {a.LExpr} for Assignment");
                }
                return new Assignment(ResolveExpr(a.LExpr, variableMap), ResolveExpr(a.RExpr, variableMap));

            case BinaryOp b:
                return new BinaryOp(b.Op, ResolveExpr(b.LExpr, variableMap), ResolveExpr(b.RExpr, variableMap));

            case Constant c:
                return c;

            case NullStatement:
                return e;

            case PostfixOp po:
                if (po.LValExpr is not Var)
                {
                    throw new ValidationError($"PostfixOp lval must be a Var, not {po.LValExpr}");
                }
                return new PostfixOp(po.Op, ResolveExpr(po.LValExpr, variableMap));

            case PrefixOp pe:
                if (pe.LValExpr is not Var)
                {
                    throw new ValidationError($"PrefixOp lval must be a Var, not {pe.LValExpr}");
                }
                return new PrefixOp(pe.Op, ResolveExpr(pe.LValExpr, variableMap));

            case Ternary t:
                return new Ternary(
                    ResolveExpr(t.CondExpr, variableMap),
                    ResolveExpr(t.Middle, variableMap),
                    ResolveExpr(t.Right, variableMap)
                );

            case UnaryOp u when u.Op.TokenType == TokenType.Minus && u.Expr is Constant c:
                return new Constant(-c.Int);

            case UnaryOp u:
                return new UnaryOp(u.Op, ResolveExpr(u.Expr, variableMap));

            case Var v:
                if (variableMap.TryGetValue(v.Name, out var globalName, out _))
                {
                    return new Var(globalName);
                }
                else
                {
                    throw new ValidationError($"Undeclared variable {v.Name}");
                }

            default:
                throw new NotImplementedException($"AST node {e} not handled yet");
        }
    }
}