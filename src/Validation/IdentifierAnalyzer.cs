using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Validation.Tools;
using Wacc.Validation.Types;

namespace Wacc.Validation;

public class IdentifierAnalyzer : BaseValidationAstRewriter
{
    internal Dictionary<string, int> UniqueVarCounters = [];

    public override CompUnit Validate(CompUnit program)
    {
        program.IdentifierMap = new IdentifierMap();
        var newAst = ResolveStatement(program, program.IdentifierMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override AstNode OnBlockStat(Block stat, IdentifierMap identMap)
        => base.OnBlockStat(stat, new IdentifierMap(identMap));

    public override AstNode OnDoLoopStat(DoLoop stat, IdentifierMap identMap)
    {
        var newMap = new IdentifierMap(identMap);
        return (DoLoop)base.OnDoLoopStat(stat, newMap) with { IdentifierMap = newMap };
    }

    public override AstNode OnForLoopStat(ForLoop stat, IdentifierMap identMap)
    {
        var newMap = new IdentifierMap(identMap);
        return (ForLoop)base.OnForLoopStat(stat, newMap) with { IdentifierMap = newMap };
    }

    public override AstNode OnFunctionDecl(FunctionDecl stat, IdentifierMap identMap)
    {
        if (identMap.TryGetValue(stat.Identifier.Name, out var entry, out var inCurScope))
        {
            if (inCurScope && !entry.HasLinkage)
            {
                throw new ValidationError($"Duplicate function declaration {stat.Identifier} at {stat.LeadToken.Location}");
            }
        }

        identMap[stat.Identifier.Name] = new MapEntry(NewName: stat.Identifier.Name, Type: new FuncType(stat.Params.Params.Count()), FromCurrentScope: true, HasLinkage: true);

        var innerMap = new IdentifierMap(identMap);

        var newParams = new List<Var>();
        foreach (var param in stat.Params.Params)
        {
            innerMap[param.Name] = new MapEntry(NewName: GenUniqueVarName(param.Name), Type: new IntType(), FromCurrentScope: true);
        }

        var newBody = stat.Body is not null ? (Block)ResolveStatement(stat.Body, innerMap) : null;

        return new FunctionDecl(new IntType(), stat.Identifier, new ParamList(newParams), newBody) { IdentifierMap = identMap };
    }

    public override AstNode OnVarDeclStat(VarDecl stat, IdentifierMap identMap)
    {
        var (declType, ident, init) = stat;
        if (identMap.TryGetValue(ident.Name, out _, out var inCurScope) && inCurScope)
        {
            throw new ValidationError($"Duplicate variable declaration for {ident}");
        }

        var uniqueName = GenUniqueVarName(ident.Name);
        identMap[ident.Name] = new MapEntry(NewName: uniqueName, Type: new IntType(), FromCurrentScope: true);

        if (init is not null)
        {
            init = ResolveExpr(init, identMap);
        }

        return new VarDecl(declType, new Var(uniqueName), init);
    }

    public override AstNode OnVarExpr(Var expr, IdentifierMap identMap)
    {
        if (identMap.TryGetValue(expr.Name, out var globalName, out _))
        {
            return new Var(globalName.NewName);
        }
        else
        {
            throw new ValidationError($"Undeclared variable {expr.Name} at {expr.LeadToken.Location}");
        }
    }

    public override AstNode OnWhileLoopStat(WhileLoop stat, IdentifierMap identMap)
    {
        var newMap = new IdentifierMap(identMap);
        return (WhileLoop)base.OnWhileLoopStat(stat, newMap) with { IdentifierMap = newMap };
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