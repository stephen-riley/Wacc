using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Validation.Tools;

namespace Wacc.Validation;

public class LoopAnalyzer : BaseValidationAstRewriter
{
    public override CompUnit Validate(CompUnit program)
    {
        var variableMap = new IdentifierMap();
        var newAst = ResolveStatement(program, variableMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override AstNode OnBlockStat(Block node, IdentifierMap variableMap)
    {
        return (Block)base.OnBlockStat(node, node.IdentifierMap!);
    }

    public override AstNode OnDoLoopStat(DoLoop stat, IdentifierMap variableMap)
    {
        var newLoopLabel = stat.IdentifierMap!.NewLoopLabel();
        return (DoLoop)base.OnDoLoopStat(stat, stat.IdentifierMap!) with { Label = newLoopLabel };
    }
    public override AstNode OnForLoopStat(ForLoop stat, IdentifierMap variableMap)
    {
        var newLoopLabel = stat.IdentifierMap!.NewLoopLabel();
        return (ForLoop)base.OnForLoopStat(stat, stat.IdentifierMap!) with { Label = newLoopLabel };
    }

    public override AstNode OnWhileLoopStat(WhileLoop stat, IdentifierMap variableMap)
    {
        var newLoopLabel = stat.IdentifierMap!.NewLoopLabel();
        return (WhileLoop)base.OnWhileLoopStat(stat, stat.IdentifierMap!) with { Label = newLoopLabel };
    }

    public override AstNode OnStatDefault(AstNode stat, IdentifierMap variableMap) => stat;

    public override AstNode OnVarExpr(Var expr, IdentifierMap variableMap)
    {
        if (variableMap.TryGetFromValues(expr.Name, out var globalName, out _))
        {
            return new Var(globalName.NewName);
        }
        else
        {
            throw new ValidationError($"Undeclared variable {expr.Name}");
        }
    }

    internal static string? ResolveLoopLabel(string? curLabel, IdentifierMap variableMap, bool makeNew = false)
    {
        if (curLabel is not null) return curLabel;

        if (makeNew) variableMap.NewLoopLabel();

        var curLoopLabel = variableMap.GetLoopLabel();
        return curLoopLabel is not null ? curLoopLabel : throw new ValidationError($"no loop currently active");
    }
}