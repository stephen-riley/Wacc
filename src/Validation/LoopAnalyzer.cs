using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Validation.Tools;

namespace Wacc.Validation;

public class LoopAnalyzer : BaseValidationAstRewriter
{
    public override CompUnit Validate(CompUnit program)
    {
        var identMap = new IdentifierMap();
        var newAst = ResolveStatement(program, identMap);
        return newAst is CompUnit unit ? unit : throw new ValidationError($"{newAst} is not a CompUnit");
    }

    public override AstNode OnBlockStat(Block node, IdentifierMap identMap)
    {
        return (Block)base.OnBlockStat(node, node.IdentifierMap!);
    }

    public override AstNode OnDoLoopStat(DoLoop stat, IdentifierMap identMap)
    {
        var newLoopLabel = stat.IdentifierMap!.NewLoopLabel();
        return (DoLoop)base.OnDoLoopStat(stat, stat.IdentifierMap!) with { Label = newLoopLabel };
    }
    public override AstNode OnForLoopStat(ForLoop stat, IdentifierMap identMap)
    {
        var newLoopLabel = stat.IdentifierMap!.NewLoopLabel();
        return (ForLoop)base.OnForLoopStat(stat, stat.IdentifierMap!) with { Label = newLoopLabel };
    }

    public override AstNode OnWhileLoopStat(WhileLoop stat, IdentifierMap identMap)
    {
        var newLoopLabel = stat.IdentifierMap!.NewLoopLabel();
        return (WhileLoop)base.OnWhileLoopStat(stat, stat.IdentifierMap!) with { Label = newLoopLabel };
    }

    public override AstNode OnStatDefault(AstNode stat, IdentifierMap identMap) => stat;

    public override AstNode OnVarExpr(Var expr, IdentifierMap identMap)
    {
        if (identMap.TryGetFromValues(expr.Name, out var globalName, out _))
        {
            return new Var(globalName.NewName);
        }
        else
        {
            throw new ValidationError($"Undeclared variable {expr.Name}");
        }
    }
}