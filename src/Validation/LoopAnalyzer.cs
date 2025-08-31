using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

public class LoopAnalyzer : BaseTreeRewriter
{
    public override CompUnit Validate(CompUnit program)
    {
        var variableMap = new VarMap();
        var newAst = ResolveStatement(program, variableMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override IAstNode OnBlockStat(Block node, VarMap variableMap)
    {
        return (Block)base.OnBlockStat(node, node.VariableMap!);
    }

    public override IAstNode OnBreakStat(Break stat, VarMap variableMap) => stat with { Label = ResolveLoopLabel(stat.Label, variableMap) };

    public override IAstNode OnContinueStat(Continue stat, VarMap variableMap) => stat with { Label = ResolveLoopLabel(stat.Label, variableMap) };

    public override IAstNode OnDoLoopStat(DoLoop stat, VarMap variableMap)
    {
        var newLoopLabel = stat.VariableMap!.NewLoopLabel();
        return (DoLoop)base.OnDoLoopStat(stat, stat.VariableMap!) with { Label = newLoopLabel };
    }
    public override IAstNode OnForLoopStat(ForLoop stat, VarMap variableMap)
    {
        var newLoopLabel = stat.VariableMap!.NewLoopLabel();
        return (ForLoop)base.OnForLoopStat(stat, stat.VariableMap!) with { Label = newLoopLabel };
    }

    public override IAstNode OnWhileLoopStat(WhileLoop stat, VarMap variableMap)
    {
        var newLoopLabel = stat.VariableMap!.NewLoopLabel();
        return (WhileLoop)base.OnWhileLoopStat(stat, stat.VariableMap!) with { Label = newLoopLabel };
    }

    public override IAstNode OnStatDefault(IAstNode stat, VarMap variableMap) => stat;

    public override IAstNode OnVarExpr(Var expr, VarMap variableMap)
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

    internal static string? ResolveLoopLabel(string? curLabel, VarMap variableMap, bool makeNew = false)
    {
        if (curLabel is not null) return curLabel;

        if (makeNew) variableMap.NewLoopLabel();

        var curLoopLabel = variableMap.GetLoopLabel();
        return curLoopLabel is not null ? curLoopLabel : throw new ValidationError($"no loop currently active");
    }
}