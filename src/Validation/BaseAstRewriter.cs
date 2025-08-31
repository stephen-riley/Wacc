using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Validation;

public class BaseAstRewriter
{
    public RuntimeState? Options { get; set; }

    public virtual CompUnit Validate(CompUnit program) => throw new ValidationError($"{GetType().Name} must override {nameof(Validate)}");

    #region StatementHandlers
    public virtual IAstNode OnAssignmentStat(Assignment stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual IAstNode OnBinaryOpStat(BinaryOp stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual IAstNode OnBlockStat(Block stat, VarMap variableMap)
    {
        var newMap = new VarMap(variableMap);
        var blockItems = new List<IAstNode>();
        foreach (var item in stat.BlockItems)
        {
            blockItems.Add(ResolveStatement(item, newMap));
        }
        var newBlock = new Block([.. blockItems]) { VariableMap = variableMap };
        return newBlock;
    }

    public virtual IAstNode OnBreakStat(Break stat, VarMap variableMap) => stat;

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

    public virtual IAstNode OnContinueStat(Continue stat, VarMap variableMap) => stat;

    public virtual IAstNode OnDeclarationStat(Declaration stat, VarMap variableMap)
    {
        var (declType, ident, init) = stat;
        if (init is not null)
        {
            init = ResolveExpr(init, variableMap);
        }
        return new Declaration(declType, ident, init);
    }

    public virtual IAstNode OnDoLoopStat(DoLoop stat, VarMap variableMap)
    {
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, variableMap);
        var body = ResolveStatement(stat.BodyBlock, variableMap);
        var newDo = new DoLoop(body, cond, variableMap.GetLoopLabel());
        return newDo with { VariableMap = variableMap };
    }

    public virtual IAstNode OnExpressionStat(Expression stat, VarMap variableMap) => new Expression(ResolveExpr(stat.SubExpr, variableMap));

    public virtual IAstNode OnForLoopStat(ForLoop stat, VarMap variableMap)
    {
        var init = ResolveStatement(stat.InitStat, variableMap);
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, variableMap);
        var post = stat.CondExpr is NullStatement ? stat.UpdateStat : ResolveExpr(stat.UpdateStat, variableMap);
        var body = ResolveStatement(stat.BodyBlock, variableMap);
        var newFor = new ForLoop(init, cond, post, body, variableMap.GetLoopLabel()) { VariableMap = variableMap };
        return newFor;
    }

    public virtual IAstNode OnFunction(Function stat, VarMap variableMap)
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

    public virtual IAstNode OnIfElseStat(IfElse stat, VarMap variableMap)
        => new IfElse(
                ResolveExpr(stat.CondExpr, variableMap),
                ResolveStatement(stat.ThenBlock, variableMap),
                stat.ElseBlock is not null ? ResolveStatement(stat.ElseBlock, variableMap) : null
            );

    public virtual IAstNode OnLabeledStatementStat(LabeledStatement stat, VarMap variableMap)
        => new LabeledStatement(
                stat.Label,
                ResolveStatement(stat.Stat, variableMap)
            );

    public virtual IAstNode OnNullStatementStat(NullStatement stat, VarMap variableMap) => stat;

    public virtual IAstNode OnPostfixOpStat(PostfixOp stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual IAstNode OnPrefixOpStat(PrefixOp stat, VarMap variableMap) => ResolveExpr(stat, variableMap);

    public virtual IAstNode OnReturnStat(Return stat, VarMap variableMap) => new Return(ResolveExpr(stat.Expr, variableMap));

    public virtual IAstNode OnTernaryStat(Ternary stat, VarMap variableMap)
        => new Ternary(
                ResolveExpr(stat.CondExpr, variableMap),
                ResolveExpr(stat.Middle, variableMap),
                ResolveExpr(stat.Right, variableMap)
            );

    public virtual IAstNode OnWhileLoopStat(WhileLoop stat, VarMap variableMap)
    {
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, variableMap);
        var body = ResolveStatement(stat.BodyBlock, variableMap);
        var newWhile = new WhileLoop(cond, body, stat.Label);
        return newWhile with { VariableMap = variableMap };
    }

    public virtual IAstNode OnStatDefault(IAstNode stat, VarMap variableMap)
        => throw new ValidationError($"{GetType().Name}.{nameof(OnStatDefault)} cannot yet handle {stat.GetType().Name}");

    #endregion

    #region ExpressionHandlers
    public virtual IAstNode OnAssignmentExpr(Assignment expr, VarMap variableMap)
    {
        if (expr.LExpr is not Var)
        {
            throw new ValidationError($"Invalid lval {expr.LExpr} for Assignment");
        }
        return new Assignment(ResolveExpr(expr.LExpr, variableMap), ResolveExpr(expr.RExpr, variableMap));
    }

    public virtual IAstNode OnBinaryOpExpr(BinaryOp expr, VarMap variableMap) => new BinaryOp(expr.Op, ResolveExpr(expr.LExpr, variableMap), ResolveExpr(expr.RExpr, variableMap));

    public virtual IAstNode OnConstantExpr(Constant expr, VarMap variableMap) => expr;

    public virtual IAstNode OnNullStatementExpr(NullStatement expr, VarMap variableMap) => expr;

    public virtual IAstNode OnPostfixOpExpr(PostfixOp expr, VarMap variableMap)
    {
        if (expr.LValExpr is not Var)
        {
            throw new ValidationError($"PostfixOp lval must be a Var, not {expr.LValExpr}");
        }
        return new PostfixOp(expr.Op, ResolveExpr(expr.LValExpr, variableMap));
    }

    public virtual IAstNode OnPrefixOpExpr(PrefixOp expr, VarMap variableMap)
    {
        if (expr.LValExpr is not Var)
        {
            throw new ValidationError($"PrefixOp lval must be a Var, not {expr.LValExpr}");
        }
        return new PrefixOp(expr.Op, ResolveExpr(expr.LValExpr, variableMap));
    }

    public virtual IAstNode OnTernaryExpr(Ternary expr, VarMap variableMap) => new Ternary(
                    ResolveExpr(expr.CondExpr, variableMap),
                    ResolveExpr(expr.Middle, variableMap),
                    ResolveExpr(expr.Right, variableMap)
                );

    public virtual IAstNode OnUnaryOpExpr(UnaryOp expr, VarMap variableMap)
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

    public virtual IAstNode OnVarExpr(Var expr, VarMap variableMap)
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

    public virtual IAstNode OnExprDefault(IAstNode expr, VarMap variableMap) => throw new NotImplementedException($"{nameof(BaseAstRewriter)}.{nameof(ResolveExpr)}: AST node {expr.GetType().Name} not handled yet");
    #endregion

    protected IAstNode ResolveStatement(IAstNode stat, VarMap variableMap)
    {
        return stat switch
        {
            Assignment => OnAssignmentStat((Assignment)stat, variableMap),
            BinaryOp => OnBinaryOpStat((BinaryOp)stat, variableMap),
            Block => OnBlockStat((Block)stat, variableMap),
            Break => OnBreakStat((Break)stat, variableMap),
            CompUnit => OnCompUnit((CompUnit)stat, variableMap),
            Continue => OnContinueStat((Continue)stat, variableMap),
            Declaration => OnDeclarationStat((Declaration)stat, variableMap),
            DoLoop => OnDoLoopStat((DoLoop)stat, variableMap),
            Expression => OnExpressionStat((Expression)stat, variableMap),
            ForLoop => OnForLoopStat((ForLoop)stat, variableMap),
            Function => OnFunction((Function)stat, variableMap),
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