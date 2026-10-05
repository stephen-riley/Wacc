using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

public class LoopAnalyzer : BaseAstRewriter
{
    public override CompUnit Validate(CompUnit program)
    {
        var variableMap = new IdentifierMap();
        var newAst = ResolveStatement(program, variableMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override AstNode OnBlockStat(Block node, IdentifierMap variableMap)
    {
        return (Block)base.OnBlockStat(node, node.VariableMap!);
    }

    public override AstNode OnDoLoopStat(DoLoop stat, IdentifierMap variableMap)
    {
        var newLoopLabel = stat.VariableMap!.NewLoopLabel();
        return (DoLoop)base.OnDoLoopStat(stat, stat.VariableMap!) with { Label = newLoopLabel };
    }
    public override AstNode OnForLoopStat(ForLoop stat, IdentifierMap variableMap)
    {
        var newLoopLabel = stat.VariableMap!.NewLoopLabel();
        return (ForLoop)base.OnForLoopStat(stat, stat.VariableMap!) with { Label = newLoopLabel };
    }

    public override AstNode OnWhileLoopStat(WhileLoop stat, IdentifierMap variableMap)
    {
        var newLoopLabel = stat.VariableMap!.NewLoopLabel();
        return (WhileLoop)base.OnWhileLoopStat(stat, stat.VariableMap!) with { Label = newLoopLabel };
    }

    public override AstNode OnStatDefault(AstNode stat, IdentifierMap variableMap) => stat;

    public override AstNode OnVarExpr(Var expr, IdentifierMap variableMap)
    {
        if (variableMap.TryGetFromValues(expr.Name, out var globalName, out _))
        {
            return new Var(globalName);
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