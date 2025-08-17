using System.Diagnostics.CodeAnalysis;
using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Validation;

public record VarAnalyzer(RuntimeState Options)
{
    internal Dictionary<string, int> UniqueVarCounters = [];

    public Ast.Program Validate(IAstNode ast)
    {
        if (ast is Ast.Program program)
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

            return new Ast.Program(newFuncs);
        }
        else
        {
            throw new ValidationError($"Top AST node must be Program, not {ast.GetType().Name}");
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
            // Ast.Expression e when e.SubExpr is Ternary => throw new ValidationError("A ternary expression cannot be a top-level statement."),
            Assignment a => ResolveExpr(a, variableMap),
            BinaryOp bo => ResolveExpr(bo, variableMap),
            Declaration d => ResolveDeclaration(d, variableMap),
            Expression e => new Expression(ResolveExpr(e.SubExpr, variableMap)),
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
            Block b => ResolveBlock(b, variableMap),
            _ => stat
        };
    }

    internal static IAstNode ResolveExpr(IAstNode e, VarMap variableMap)
    {
        switch (e)
        {
            case Constant c:
                return c;

            case Assignment a:
                if (a.LExpr is not Var)
                {
                    throw new ValidationError($"Invalid lval {a.LExpr} for Assignment");
                }
                return new Assignment(ResolveExpr(a.LExpr, variableMap), ResolveExpr(a.RExpr, variableMap));

            case Var v:
                if (variableMap.TryGetValue(v.Name, out var globalName, out _))
                {
                    return new Var(globalName);
                }
                else
                {
                    throw new ValidationError($"Undeclared variable {v.Name}");
                }

            case BinaryOp b:
                return new BinaryOp(b.Op, ResolveExpr(b.LExpr, variableMap), ResolveExpr(b.RExpr, variableMap));

            case UnaryOp u when u.Op.TokenType == TokenType.Minus && u.Expr is Constant c:
                return new Constant(-c.Int);

            case UnaryOp u:
                return new UnaryOp(u.Op, ResolveExpr(u.Expr, variableMap));

            case PrefixOp pe:
                if (pe.LValExpr is not Var)
                {
                    throw new ValidationError($"PrefixOp lval must be a Var, not {pe.LValExpr}");
                }
                return new PrefixOp(pe.Op, ResolveExpr(pe.LValExpr, variableMap));

            case PostfixOp po:
                if (po.LValExpr is not Var)
                {
                    throw new ValidationError($"PostfixOp lval must be a Var, not {po.LValExpr}");
                }
                return new PostfixOp(po.Op, ResolveExpr(po.LValExpr, variableMap));

            case Ternary t:
                return new Ternary(
                    ResolveExpr(t.CondExpr, variableMap),
                    ResolveExpr(t.Middle, variableMap),
                    ResolveExpr(t.Right, variableMap)
                );

            default:
                throw new NotImplementedException($"AST node {e} not handled yet");
        }
    }

    internal class VarMap
    {
        public VarMap() { }
        public VarMap(VarMap m)
        {
            Map = [];
            Parent = m;
        }

        private readonly Dictionary<string, string> Map = [];
        public VarMap? Parent = null;
        public bool ContainsKey(string key) => Map.ContainsKey(key);
        public string this[string key]
        {
            get => Map[key];
            set => Map[key] = value;
        }
        public bool TryGetValue(string key, [NotNullWhen(true)] out string value, out bool inCurScope)
        {
            inCurScope = false;
            var scope = this;

            do
            {
                if (scope.Map.TryGetValue(key, out var v))
                {
                    value = v;
                    inCurScope = scope == this;
                    return true;
                }
                scope = scope.Parent;
            } while (scope is not null);

            value = null!;
            return false;
        }
    }

    internal Block ResolveBlock(Block block, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        var blockItems = new List<IAstNode>();
        foreach (var item in block.BlockItems)
        {
            blockItems.Add(ResolveStatement(item, newMap));
        }
        var newBlock = new Block([.. blockItems]);
        return newBlock;
    }
}