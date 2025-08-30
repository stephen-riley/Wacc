using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

// TODO: experimental and not currently used.  See `BaseTreeRewriter` for details.

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
        var newMap = new VarMap(variableMap);
        var blockItems = new List<IAstNode>();
        foreach (var item in node.BlockItems)
        {
            blockItems.Add(ResolveStatement(item, newMap));
        }
        var newBlock = new Block([.. blockItems]);
        return newBlock;
    }

    public override IAstNode OnBreakStat(Break stat, VarMap variableMap) => stat with { Label = ResolveLoopLabel(stat.Label, variableMap) };
    public override IAstNode OnContinueStat(Continue stat, VarMap variableMap) => stat with { Label = ResolveLoopLabel(stat.Label, variableMap) };
    public override IAstNode OnDoLoopStat(DoLoop stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        var newLoopLabel = newMap.NewLoopLabel();
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, newMap);
        var body = ResolveStatement(stat.BodyBlock, newMap);
        var newDo = new DoLoop(body, cond, newLoopLabel);
        return newDo;
    }
    public override IAstNode OnForLoopStat(ForLoop stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        var newLoopLabel = newMap.NewLoopLabel();
        var init = ResolveStatement(stat.InitStat, newMap);
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, newMap);
        var post = stat.CondExpr is NullStatement ? stat.UpdateStat : ResolveExpr(stat.UpdateStat, newMap);
        var body = ResolveStatement(stat.BodyBlock, newMap);
        var newFor = new ForLoop(init, cond, post, body, newLoopLabel);
        return newFor;
    }

    public override IAstNode OnWhileLoopStat(WhileLoop stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        var newLoopLabel = newMap.NewLoopLabel();
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, newMap);
        var body = ResolveStatement(stat.BodyBlock, newMap);
        var newWhile = new DoLoop(cond, body, newLoopLabel);
        return newWhile;
    }

    internal static string? ResolveLoopLabel(string? curLabel, VarMap variableMap, bool makeNew = false)
    {
        if (curLabel is not null) return curLabel;

        if (makeNew) variableMap.NewLoopLabel();

        var curLoopLabel = variableMap.GetLoopLabel();
        return curLoopLabel is not null ? curLoopLabel : throw new ValidationError($"no loop currently active");
    }
}