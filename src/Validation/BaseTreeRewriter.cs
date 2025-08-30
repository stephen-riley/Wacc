using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

// TODO: this is experimental and currently unused.  Lots of problems with this, including it not
//  exploring the children of each node.

public class BaseTreeRewriter
{
    public bool DefaultToError { get; set; }

    public RuntimeState? Options { get; set; }

    public virtual CompUnit Validate(CompUnit program) => throw new ValidationError($"{GetType().Name} must override {nameof(Validate)}");

    public virtual IAstNode OnAssignmentStat(Assignment stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Assignment)}")
            : stat;
    }

    public virtual IAstNode OnBinaryOpStat(BinaryOp stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(BinaryOp)}")
            : stat;
    }

    public virtual IAstNode OnBlockStat(Block stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Block)}")
            : stat;
    }

    public virtual IAstNode OnBreakStat(Break stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Break)}")
            : stat;
    }

    public virtual IAstNode OnContinueStat(Continue stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Continue)}")
            : stat;
    }

    public virtual IAstNode OnDeclarationStat(Declaration stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Declaration)}")
            : stat;
    }

    public virtual IAstNode OnDoLoopStat(DoLoop stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(DoLoop)}")
            : stat;
    }

    public virtual IAstNode OnExpressionStat(Expression stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Expression)}")
            : stat;
    }

    public virtual IAstNode OnForLoopStat(ForLoop stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(ForLoop)}")
            : stat;
    }

    public virtual IAstNode OnIfElseStat(IfElse stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(IfElse)}")
            : stat;
    }

    public virtual IAstNode OnLabeledStatementStat(LabeledStatement stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(LabeledStatement)}")
            : stat;
    }

    public virtual IAstNode OnPostfixOpStat(PostfixOp stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(PostfixOp)}")
            : stat;
    }

    public virtual IAstNode OnPrefixOpStat(PrefixOp stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(PrefixOp)}")
            : stat;
    }

    public virtual IAstNode OnReturnStat(Return stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Return)}")
            : stat;
    }

    public virtual IAstNode OnTernaryStat(Ternary stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Ternary)}")
            : stat;
    }

    public virtual IAstNode OnWhileLoopStat(WhileLoop stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(WhileLoop)}")
            : stat;
    }

    public virtual IAstNode OnStatDefault(IAstNode stat, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(OnStatDefault)}")
            : stat;
    }

    public virtual IAstNode OnBinaryOpExpr(BinaryOp expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(BinaryOp)}")
            : expr;
    }

    public virtual IAstNode OnConstantExpr(Constant expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Constant)}")
            : expr;
    }

    public virtual IAstNode OnNullStatementExpr(NullStatement expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(NullStatement)}")
            : expr;
    }

    public virtual IAstNode OnPostfixOpExpr(PostfixOp expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(PostfixOp)}")
            : expr;
    }

    public virtual IAstNode OnPrefixOpExpr(PrefixOp expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(PrefixOp)}")
            : expr;
    }

    public virtual IAstNode OnTernaryExpr(Ternary expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Ternary)}")
            : expr;
    }

    public virtual IAstNode OnUnaryOpExpr(UnaryOp expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(UnaryOp)}")
            : expr;
    }

    public virtual IAstNode OnVarExpr(Var expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {GetType().Name}.{nameof(Var)}")
            : expr;
    }

    public virtual IAstNode OnExprDefault(IAstNode expr, VarMap variableMap)
    {
        return DefaultToError
            ? throw new ValidationError($"no override for {this.GetType().Name}.{nameof(OnExprDefault)}")
            : expr;
    }

    protected IAstNode ResolveStatement(IAstNode stat, VarMap variableMap)
    {
        return stat switch
        {
            Assignment => OnAssignmentStat((Assignment)stat, variableMap),
            BinaryOp => OnBinaryOpStat((BinaryOp)stat, variableMap),
            Block => OnBlockStat((Block)stat, variableMap),
            Break => OnBreakStat((Break)stat, variableMap),
            Continue => OnContinueStat((Continue)stat, variableMap),
            Declaration => OnDeclarationStat((Declaration)stat, variableMap),
            DoLoop => OnDoLoopStat((DoLoop)stat, variableMap),
            Expression => OnExpressionStat((Expression)stat, variableMap),
            ForLoop => OnForLoopStat((ForLoop)stat, variableMap),
            IfElse => OnIfElseStat((IfElse)stat, variableMap),
            LabeledStatement => OnLabeledStatementStat((LabeledStatement)stat, variableMap),
            PostfixOp => OnPostfixOpStat((PostfixOp)stat, variableMap),
            PrefixOp => OnPrefixOpStat((PrefixOp)stat, variableMap),
            Return => OnReturnStat((Return)stat, variableMap),
            Ternary => OnTernaryStat((Ternary)stat, variableMap),
            WhileLoop => OnWhileLoopStat((WhileLoop)stat, variableMap),
            _ => OnStatDefault(stat, variableMap)
        };
    }

    protected IAstNode ResolveExpr(IAstNode expr, VarMap variableMap)
    {
        return expr switch
        {
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