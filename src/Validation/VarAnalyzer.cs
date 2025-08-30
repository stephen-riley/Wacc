using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Validation;

public record VarAnalyzer(RuntimeState Options)
{
    internal Dictionary<string, int> UniqueVarCounters = [];

    public CompUnit Validate(CompUnit program)
    {
        var newFuncs = new List<Function>();

        foreach (var func in program.Functions)
        {
            var funcVariableMap = new VarMap();
            var newStats = func.Body.BlockItems.Select(stat =>
            {
                var newStat = ResolveStatement(stat, funcVariableMap);
                return newStat;
            });
            newFuncs.Add(new Function(func.Type, func.Name, new Block([.. newStats])));
        }

        return new CompUnit(newFuncs);
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

    internal Declaration ResolveDeclaration(Declaration decl, VarMap variableMap)
    {
        var (declType, ident, init) = decl;

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

    internal IAstNode ResolveStatement(IAstNode stat, VarMap variableMap)
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
                var newBlock = new Block([.. blockItems]);
                return newBlock;
            }),
            Break br => br with { Label = ResolveLoopLabel(br.Label, variableMap) },
            Continue c => c with { Label = ResolveLoopLabel(c.Label, variableMap) },
            Declaration d => ResolveDeclaration(d, variableMap),
            DoLoop dl => Ext.Do(() =>
            {
                var newMap = new VarMap(variableMap);
                var newLoopLabel = newMap.NewLoopLabel();
                var cond = dl.CondExpr is NullStatement ? dl.CondExpr : ResolveExpr(dl.CondExpr, newMap);
                var body = ResolveStatement(dl.BodyBlock, newMap);
                var newDo = new DoLoop(body, cond, newLoopLabel);
                return newDo;
            }),
            Expression e => new Expression(ResolveExpr(e.SubExpr, variableMap)),
            ForLoop fl => Ext.Do(() =>
            {
                var newMap = new VarMap(variableMap);
                var newLoopLabel = newMap.NewLoopLabel();
                var init = ResolveStatement(fl.InitStat, newMap);
                var cond = fl.CondExpr is NullStatement ? fl.CondExpr : ResolveExpr(fl.CondExpr, newMap);
                var post = fl.CondExpr is NullStatement ? fl.UpdateStat : ResolveExpr(fl.UpdateStat, newMap);
                var body = ResolveStatement(fl.BodyBlock, newMap);
                var newFor = new ForLoop(init, cond, post, body, newLoopLabel);
                return newFor;
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
                var newLoopLabel = newMap.NewLoopLabel();
                var cond = wl.CondExpr is NullStatement ? wl.CondExpr : ResolveExpr(wl.CondExpr, newMap);
                var body = ResolveStatement(wl.BodyBlock, newMap);
                var newWhile = new DoLoop(cond, body, newLoopLabel);
                return newWhile;
            }),
            _ => stat
        };
    }

    internal static IAstNode ResolveExpr(IAstNode e, VarMap variableMap)
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

    internal static string? ResolveLoopLabel(string? curLabel, VarMap variableMap, bool makeNew = false)
    {
        if (curLabel is not null) return curLabel;

        if (makeNew) variableMap.NewLoopLabel();

        var curLoopLabel = variableMap.GetLoopLabel();
        return curLoopLabel is not null ? curLoopLabel : throw new ValidationError($"no loop currently active");
    }
}