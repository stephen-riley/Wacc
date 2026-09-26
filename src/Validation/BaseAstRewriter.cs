using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Validation;

public class BaseAstRewriter
{
    public RuntimeState? Options { get; set; }

    public virtual CompUnit Validate(CompUnit program) => throw new ValidationError($"{GetType().Name} must override {nameof(Validate)}");

    #region StatementHandlers
    public virtual AstNode OnAssignmentStat(Assignment stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual AstNode OnBinaryOpStat(BinaryOp stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual AstNode OnBlockStat(Block stat, VarMap variableMap)
    {
        var blockItems = new List<AstNode>();
        foreach (var item in stat.BlockItems)
        {
            blockItems.Add(ResolveStatement(item, variableMap));
        }
        var newBlock = new Block([.. blockItems]) { VariableMap = variableMap };
        return newBlock;
    }

    public virtual AstNode OnBreakStat(Break stat, VarMap variableMap) => stat;

    public virtual AstNode OnCase(Case stat, VarMap variableMap)
        => new Case(
            ResolveExpr(stat.CaseCondExpr, variableMap)
        // ResolveStatement(stat.CaseBlock, variableMap)
        );

    public virtual CompUnit OnCompUnit(CompUnit stat, VarMap variableMap)
    {
        var newFuncs = new List<Function>();
        foreach (var func in stat.Functions)
        {
            var nf = (Function)ResolveStatement(func, variableMap);
            newFuncs.Add(nf);
        }
        return new CompUnit([.. newFuncs]);
    }

    public virtual AstNode OnContinueStat(Continue stat, VarMap variableMap) => stat;

    public virtual AstNode OnDeclarationStat(Declaration stat, VarMap variableMap)
    {
        var (declType, ident, init) = stat;
        if (init is not null)
        {
            init = ResolveExpr(init, variableMap);
        }
        return new Declaration(declType, ident, init);
    }

    public virtual AstNode OnDefault(Default stat, VarMap variableMap)
        => new Default(
        // ResolveStatement(stat.DefaultBlock, variableMap)
        );

    public virtual AstNode OnDoLoopStat(DoLoop stat, VarMap variableMap)
    {
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, variableMap);
        var body = ResolveStatement(stat.BodyBlock, variableMap);
        var newDo = new DoLoop(body, cond, variableMap.GetLoopLabel());
        return newDo with { VariableMap = variableMap };
    }

    public virtual AstNode OnExpressionStat(Expression stat, VarMap variableMap) => new Expression(ResolveExpr(stat.SubExpr, variableMap));

    public virtual AstNode OnForLoopStat(ForLoop stat, VarMap variableMap)
    {
        var init = ResolveStatement(stat.InitStat, variableMap);
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, variableMap);
        var post = stat.PostStat is NullStatement ? stat.PostStat : ResolveExpr(stat.PostStat, variableMap);
        var body = ResolveStatement(stat.BodyBlock, variableMap);
        var newFor = new ForLoop(init, cond, post, body, variableMap.GetLoopLabel()) { VariableMap = variableMap };
        return newFor;
    }

    public virtual AstNode OnFunction(Function stat, VarMap variableMap)
    {
        var body = ResolveStatement(stat.Body, stat.Body.VariableMap!);
        if (body is Block b)
        {
            return new Function(stat.Type, stat.Name, b) with { VariableMap = stat.VariableMap };
        }
        else
        {
            throw new ValidationError($"Function {stat.Name} body must be a block");
        }
    }

    public virtual AstNode OnIfElseStat(IfElse stat, VarMap variableMap)
        => new IfElse(
                ResolveExpr(stat.CondExpr, variableMap),
                ResolveStatement(stat.ThenBlock, variableMap),
                stat.ElseBlock is not null ? ResolveStatement(stat.ElseBlock, variableMap) : null
            );

    public virtual AstNode OnGotoStat(Goto stat, VarMap variableMap) => stat;

    public virtual AstNode OnLabeledStatementStat(LabeledBlock stat, VarMap variableMap)
        => new LabeledBlock(
                stat.Label,
                ResolveStatement(stat.Stat, variableMap)
            );

    public virtual AstNode OnNullStatementStat(NullStatement stat, VarMap variableMap) => stat;

    public virtual AstNode OnPostfixOpStat(PostfixOp stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual AstNode OnPrefixOpStat(PrefixOp stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual AstNode OnReturnStat(Return stat, VarMap variableMap) => new Return(ResolveExpr(stat.Expr, variableMap));

    public virtual AstNode OnSwitch(Switch stat, VarMap variableMap)
        => new Switch(
                ResolveExpr(stat.CondExpr, variableMap),
                ResolveStatement(stat.SwitchBlock, variableMap)
            );

    public virtual AstNode OnTernaryStat(Ternary stat, VarMap variableMap)
        => new Ternary(
                ResolveExpr(stat.CondExpr, variableMap),
                ResolveExpr(stat.Middle, variableMap),
                ResolveExpr(stat.Right, variableMap)
            );

    public virtual AstNode OnWhileLoopStat(WhileLoop stat, VarMap variableMap)
    {
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, variableMap);
        var body = ResolveStatement(stat.BodyBlock, variableMap);
        var newWhile = new WhileLoop(cond, body, stat.Label);
        return newWhile with { VariableMap = variableMap };
    }

    public virtual AstNode OnStatDefault(AstNode stat, VarMap variableMap)
        => throw new ValidationError($"AST node {stat.GetType().Name} not handled yet");

    #endregion

    #region ExpressionHandlers
    public virtual AstNode OnAssignmentExpr(Assignment expr, VarMap variableMap)
    {
        if (expr.LExpr is not Var)
        {
            throw new ValidationError($"Invalid lval {expr.LExpr} for Assignment");
        }
        return new Assignment(ResolveExpr(expr.LExpr, variableMap), ResolveExpr(expr.RExpr, variableMap));
    }

    public virtual AstNode OnBinaryOpExpr(BinaryOp expr, VarMap variableMap) => new BinaryOp(expr.Op, ResolveExpr(expr.LExpr, variableMap), ResolveExpr(expr.RExpr, variableMap));

    public virtual AstNode OnConstantExpr(Constant expr, VarMap variableMap) => expr;

    public virtual AstNode OnNullStatementExpr(NullStatement expr, VarMap variableMap) => expr;

    public virtual AstNode OnPostfixOpExpr(PostfixOp expr, VarMap variableMap)
    {
        if (expr.LValExpr is not Var)
        {
            throw new ValidationError($"PostfixOp lval must be a Var, not {expr.LValExpr}");
        }
        return new PostfixOp(expr.Op, ResolveExpr(expr.LValExpr, variableMap));
    }

    public virtual AstNode OnPrefixOpExpr(PrefixOp expr, VarMap variableMap)
    {
        if (expr.LValExpr is not Var)
        {
            throw new ValidationError($"PrefixOp lval must be a Var, not {expr.LValExpr}");
        }
        return new PrefixOp(expr.Op, ResolveExpr(expr.LValExpr, variableMap));
    }

    public virtual AstNode OnTernaryExpr(Ternary expr, VarMap variableMap) => new Ternary(
                    ResolveExpr(expr.CondExpr, variableMap),
                    ResolveExpr(expr.Middle, variableMap),
                    ResolveExpr(expr.Right, variableMap)
                );

    public virtual AstNode OnUnaryOpExpr(UnaryOp expr, VarMap variableMap)
    {
        if (expr.Op.TokenType == TokenType.Minus && expr.Expr is Constant c)
        {
            return new Constant(-c.Int);
        }
        else
        {
            return new UnaryOp(expr.Op, ResolveExpr(expr.Expr, variableMap));
        }
    }

    public virtual AstNode OnVarExpr(Var expr, VarMap variableMap) => expr;

    public virtual AstNode OnExprDefault(AstNode expr, VarMap variableMap) => throw new ValidationError($"AST node {expr.GetType().Name} not handled yet");
    #endregion

    protected AstNode ResolveStatement(AstNode stat, VarMap variableMap)
    {
        return stat switch
        {
            Assignment => OnAssignmentStat((Assignment)stat, variableMap),
            BinaryOp => OnBinaryOpStat((BinaryOp)stat, variableMap),
            Block => OnBlockStat((Block)stat, variableMap),
            Break => OnBreakStat((Break)stat, variableMap),
            Case => OnCase((Case)stat, variableMap),
            CompUnit => OnCompUnit((CompUnit)stat, variableMap),
            Continue => OnContinueStat((Continue)stat, variableMap),
            Declaration => OnDeclarationStat((Declaration)stat, variableMap),
            Default => OnDefault((Default)stat, variableMap),
            DoLoop => OnDoLoopStat((DoLoop)stat, variableMap),
            Expression => OnExpressionStat((Expression)stat, variableMap),
            ForLoop => OnForLoopStat((ForLoop)stat, variableMap),
            Function => OnFunction((Function)stat, variableMap),
            Goto => OnGotoStat((Goto)stat, variableMap),
            IfElse => OnIfElseStat((IfElse)stat, variableMap),
            LabeledBlock => OnLabeledStatementStat((LabeledBlock)stat, variableMap),
            NullStatement => OnNullStatementExpr((NullStatement)stat, variableMap),
            PostfixOp => OnPostfixOpStat((PostfixOp)stat, variableMap),
            PrefixOp => OnPrefixOpStat((PrefixOp)stat, variableMap),
            Return => OnReturnStat((Return)stat, variableMap),
            Switch => OnSwitch((Switch)stat, variableMap),
            Ternary => OnTernaryStat((Ternary)stat, variableMap),
            WhileLoop => OnWhileLoopStat((WhileLoop)stat, variableMap),
            _ => OnStatDefault(stat, variableMap)
        };
    }

    protected AstNode ResolveExpr(AstNode expr, VarMap variableMap)
    {
        return expr switch
        {
            Assignment => OnAssignmentExpr((Assignment)expr, variableMap),
            BinaryOp => OnBinaryOpExpr((BinaryOp)expr, variableMap),
            Constant => OnConstantExpr((Constant)expr, variableMap),
            NullStatement => OnNullStatementExpr((NullStatement)expr, variableMap),
            PostfixOp => OnPostfixOpExpr((PostfixOp)expr, variableMap),
            PrefixOp => OnPrefixOpExpr((PrefixOp)expr, variableMap),
            Ternary => OnTernaryExpr((Ternary)expr, variableMap),
            UnaryOp => OnUnaryOpExpr((UnaryOp)expr, variableMap),
            Var => OnVarExpr((Var)expr, variableMap),
            _ => OnExprDefault(expr, variableMap),
        };
    }
}