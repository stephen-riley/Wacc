using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

public class IdentifierAnalyzer : BaseAstRewriter
{
    internal Dictionary<string, int> UniqueVarCounters = [];

    public override CompUnit Validate(CompUnit program)
    {
        var variableMap = new IdentifierMap();
        var newAst = ResolveStatement(program, variableMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override AstNode OnBlockStat(Block stat, IdentifierMap variableMap)
        => base.OnBlockStat(stat, new IdentifierMap(variableMap));

    public override AstNode OnDoLoopStat(DoLoop stat, IdentifierMap variableMap)
    {
        var newMap = new IdentifierMap(variableMap);
        return (DoLoop)base.OnDoLoopStat(stat, newMap) with { VariableMap = newMap };
    }

    public override AstNode OnForLoopStat(ForLoop stat, IdentifierMap variableMap)
    {
        var newMap = new IdentifierMap(variableMap);
        return (ForLoop)base.OnForLoopStat(stat, newMap) with { VariableMap = newMap };
    }

    public override AstNode OnFunctionDecl(FunctionDecl stat, IdentifierMap variableMap)
    {
        var newMap = new IdentifierMap(variableMap);
        return (FunctionDecl)base.OnFunctionDecl(stat, newMap) with { VariableMap = newMap };
    }

    public override AstNode OnVarDeclStat(VarDecl stat, IdentifierMap variableMap)
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

        return new VarDecl(declType, new Var(uniqueName), init);
    }

    public override AstNode OnWhileLoopStat(WhileLoop stat, IdentifierMap variableMap)
    {
        var newMap = new IdentifierMap(variableMap);
        return (WhileLoop)base.OnWhileLoopStat(stat, newMap) with { VariableMap = newMap };
    }

    public override AstNode OnVarExpr(Var expr, IdentifierMap variableMap)
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
}