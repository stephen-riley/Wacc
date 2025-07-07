using System.Linq.Expressions;
using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tokens;
using VarMap = System.Collections.Generic.Dictionary<string, string>;

namespace Wacc.Analyzers;

public class SemanticAnalyzer(RuntimeState opts)
{
    public RuntimeState Options = opts;

    internal VarMap VariableMap = [];

    internal Dictionary<string, int> UniqueVarCounters = [];

    public bool Execute()
    {
        Options.Ast = Validate(Options.Ast);

        if (Options.Verbose || Options.OnlyThroughLexer)
        {
            if (Options.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("SEMANTIC VALIDATION:");
                Console.Error.WriteLine("=====================");

                var stream = Options.Verbose ? Console.Error : Console.Out;
                stream.WriteLine(Options.Ast.ToPrettyString());
            }
        }

        return true;
    }

    public Ast.Program Validate(IAstNode ast)
    {
        if (ast is Ast.Program program)
        {
            var newFuncs = new List<Function>();

            foreach (var func in program.Functions)
            {
                var funcVariableMap = new VarMap();
                var newStats = func.Body.Select(stat =>
                {
                    var newStat = ResolveStatement(stat, funcVariableMap);
                    return newStat;
                });
                newFuncs.Add(new Function(func.Type, func.Name, [.. newStats]));
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

        if (variableMap.ContainsKey(ident.Name))
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

    internal BlockItem ResolveStatement(BlockItem stat, VarMap variableMap)
    {
        return stat switch
        {
            Assignment a => (BlockItem)ResolveExpr(a, variableMap),
            Declaration d => ResolveDeclaration(d, variableMap),
            Return r => new Return(ResolveExpr(r.Expr, variableMap)),
            // Ast.Expression e when e.SubExpr is Ternary => throw new ValidationError("A ternary expression cannot be a top-level statement."),
            Ast.Expression e => new Ast.Expression(ResolveExpr(e.SubExpr, variableMap)),
            IfElse ie => new IfElse(
                ResolveExpr(ie.CondExpr, variableMap),
                ResolveStatement(ie.ThenStat, variableMap),
                ie.ElseStat is not null ? ResolveStatement(ie.ElseStat, variableMap) : null
            ),
            NullStatement => stat,
            _ => stat
        };
    }

    internal IAstNode ResolveExpr(IAstNode e, VarMap variableMap)
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
                if (variableMap.TryGetValue(v.Name, out var globalName))
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
}