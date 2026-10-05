using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Validation;

public class BaseAstRewriter
{
    public RuntimeState? Options { get; set; }

    public virtual CompUnit Validate(CompUnit program) => throw new ValidationError($"{GetType().Name} must override {nameof(Validate)}");

    #region StatementHandlers
    public virtual AstNode OnAssignmentStat(Assignment stat, IdentifierMap identMap) => ResolveExpr(stat, identMap);

    public virtual AstNode OnBinaryOpStat(BinaryOp stat, IdentifierMap identMap) => ResolveExpr(stat, identMap);

    public virtual AstNode OnBlockStat(Block stat, IdentifierMap identMap)
    {
        var blockItems = new List<AstNode>();
        foreach (var item in stat.BlockItems)
        {
            blockItems.Add(ResolveStatement(item, identMap));
        }
        var newBlock = new Block([.. blockItems]) { VariableMap = identMap };
        return newBlock;
    }

    public virtual AstNode OnBreakStat(Break stat, IdentifierMap identMap) => stat;

    public virtual AstNode OnCase(Case stat, IdentifierMap identMap)
        => new Case(
            ResolveExpr(stat.CaseCondExpr, identMap)
        // ResolveStatement(stat.CaseBlock, identMap)
        );

    public virtual CompUnit OnCompUnit(CompUnit stat, IdentifierMap identMap)
    {
        var newFuncs = new List<FunctionDecl>();
        foreach (var func in stat.Functions)
        {
            var nf = (FunctionDecl)ResolveStatement(func, identMap);
            newFuncs.Add(nf);
        }
        return new CompUnit([.. newFuncs]);
    }

    public virtual AstNode OnContinueStat(Continue stat, IdentifierMap identMap) => stat;

    public virtual AstNode OnDefault(Default stat, IdentifierMap identMap)
        => new Default(
        // ResolveStatement(stat.DefaultBlock, identMap)
        );

    public virtual AstNode OnDoLoopStat(DoLoop stat, IdentifierMap identMap)
    {
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, identMap);
        var body = ResolveStatement(stat.BodyBlock, identMap);
        var newDo = new DoLoop(body, cond, identMap.GetLoopLabel());
        return newDo with { VariableMap = identMap };
    }

    public virtual AstNode OnExpressionStat(Expression stat, IdentifierMap identMap) => new Expression(ResolveExpr(stat.SubExpr, identMap));

    public virtual AstNode OnForLoopStat(ForLoop stat, IdentifierMap identMap)
    {
        var init = ResolveStatement(stat.InitStat, identMap);
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, identMap);
        var post = stat.PostStat is NullStatement ? stat.PostStat : ResolveExpr(stat.PostStat, identMap);
        var body = ResolveStatement(stat.BodyBlock, identMap);
        var newFor = new ForLoop(init, cond, post, body, identMap.GetLoopLabel()) { VariableMap = identMap };
        return newFor;
    }

    public virtual AstNode OnFunctionDecl(FunctionDecl stat, IdentifierMap identMap)
    {
        var body = default(Block);

        if (stat.Body is not null)
        {
            body = (Block?)ResolveStatement(stat.Body, stat.Body.VariableMap!);
            return new FunctionDecl(stat.FuncType, stat.Identifier, stat.Params, body) with { VariableMap = stat.VariableMap };
        }
        else
        {
            return new FunctionDecl(stat.FuncType, stat.Identifier, stat.Params) with { VariableMap = stat.VariableMap };
        }
    }

    public virtual AstNode OnIfElseStat(IfElse stat, IdentifierMap identMap)
        => new IfElse(
                ResolveExpr(stat.CondExpr, identMap),
                ResolveStatement(stat.ThenBlock, identMap),
                stat.ElseBlock is not null ? ResolveStatement(stat.ElseBlock, identMap) : null
            );

    public virtual AstNode OnGotoStat(Goto stat, IdentifierMap identMap) => stat;

    public virtual AstNode OnLabeledStatementStat(LabeledBlock stat, IdentifierMap identMap)
        => new LabeledBlock(
                stat.Label,
                ResolveStatement(stat.Stat, identMap)
            );

    public virtual AstNode OnNullStatementStat(NullStatement stat, IdentifierMap identMap) => stat;

    public virtual AstNode OnParamListStat(ParamList stat, IdentifierMap identMap) => stat;

    public virtual AstNode OnPostfixOpStat(PostfixOp stat, IdentifierMap identMap) => ResolveExpr(stat, identMap);

    public virtual AstNode OnPrefixOpStat(PrefixOp stat, IdentifierMap identMap) => ResolveExpr(stat, identMap);

    public virtual AstNode OnReturnStat(Return stat, IdentifierMap identMap) => new Return(ResolveExpr(stat.Expr, identMap));

    public virtual AstNode OnSwitch(Switch stat, IdentifierMap identMap)
        => new Switch(
                ResolveExpr(stat.CondExpr, identMap),
                ResolveStatement(stat.SwitchBlock, identMap)
            );

    public virtual AstNode OnTernaryStat(Ternary stat, IdentifierMap identMap)
        => new Ternary(
                ResolveExpr(stat.CondExpr, identMap),
                ResolveExpr(stat.Middle, identMap),
                ResolveExpr(stat.Right, identMap)
            );

    public virtual AstNode OnVarDeclStat(VarDecl stat, IdentifierMap identMap)
    {
        var (declType, ident, init) = stat;
        if (init is not null)
        {
            init = ResolveExpr(init, identMap);
        }
        return new VarDecl(declType, ident, init);
    }

    public virtual AstNode OnWhileLoopStat(WhileLoop stat, IdentifierMap identMap)
    {
        var cond = stat.CondExpr is NullStatement ? stat.CondExpr : ResolveExpr(stat.CondExpr, identMap);
        var body = ResolveStatement(stat.BodyBlock, identMap);
        var newWhile = new WhileLoop(cond, body, stat.Label);
        return newWhile with { VariableMap = identMap };
    }

    public virtual AstNode OnStatDefault(AstNode stat, IdentifierMap identMap)
        => throw new ValidationError($"AST node {stat.GetType().Name} not handled yet");

    #endregion

    #region ExpressionHandlers
    public virtual AstNode OnAssignmentExpr(Assignment expr, IdentifierMap identMap)
    {
        if (expr.LExpr is not Var)
        {
            throw new ValidationError($"Invalid lval {expr.LExpr} for Assignment");
        }
        return new Assignment(ResolveExpr(expr.LExpr, identMap), ResolveExpr(expr.RExpr, identMap));
    }

    public virtual AstNode OnBinaryOpExpr(BinaryOp expr, IdentifierMap identMap) => new BinaryOp(expr.Op, ResolveExpr(expr.LExpr, identMap), ResolveExpr(expr.RExpr, identMap));

    public virtual AstNode OnConstantExpr(Constant expr, IdentifierMap identMap) => expr;

    public virtual AstNode OnFunctionCallExpr(FunctionCall fc, IdentifierMap identMap)
    {
        if (identMap.ContainsKey(fc.Identifier.Name))
        {
            var newName = fc.Identifier.Name;   // TODO: listing 9-18 has `new_name` in it--need to see what that's about
            var newArgs = new ArgumentList([.. fc.Args.Params.Select(arg => ResolveExpr(arg, identMap))]);
            return new FunctionCall(new Var(newName), newArgs);
        }
        else
        {
            throw new ValidationError($"Function call {fc.Identifier.Name} not found in identifier map");
        }
    }

    public virtual AstNode OnNullStatementExpr(NullStatement expr, IdentifierMap identMap) => expr;

    public virtual AstNode OnPostfixOpExpr(PostfixOp expr, IdentifierMap identMap)
    {
        if (expr.LValExpr is not Var)
        {
            throw new ValidationError($"PostfixOp lval must be a Var, not {expr.LValExpr}");
        }
        return new PostfixOp(expr.Op, ResolveExpr(expr.LValExpr, identMap));
    }

    public virtual AstNode OnPrefixOpExpr(PrefixOp expr, IdentifierMap identMap)
    {
        if (expr.LValExpr is not Var)
        {
            throw new ValidationError($"PrefixOp lval must be a Var, not {expr.LValExpr}");
        }
        return new PrefixOp(expr.Op, ResolveExpr(expr.LValExpr, identMap));
    }

    public virtual AstNode OnTernaryExpr(Ternary expr, IdentifierMap identMap) => new Ternary(
                    ResolveExpr(expr.CondExpr, identMap),
                    ResolveExpr(expr.Middle, identMap),
                    ResolveExpr(expr.Right, identMap)
                );

    public virtual AstNode OnUnaryOpExpr(UnaryOp expr, IdentifierMap identMap)
    {
        if (expr.Op.TokenType == TokenType.Minus && expr.Expr is Constant c)
        {
            return new Constant(-c.Int);
        }
        else
        {
            return new UnaryOp(expr.Op, ResolveExpr(expr.Expr, identMap));
        }
    }

    public virtual AstNode OnVarExpr(Var expr, IdentifierMap identMap) => expr;

    public virtual AstNode OnExprDefault(AstNode expr, IdentifierMap identMap) => throw new ValidationError($"AST node {expr.GetType().Name} not handled yet");
    #endregion

    protected AstNode ResolveStatement(AstNode stat, IdentifierMap identMap)
    {
        return stat switch
        {
            Assignment => OnAssignmentStat((Assignment)stat, identMap),
            BinaryOp => OnBinaryOpStat((BinaryOp)stat, identMap),
            Block => OnBlockStat((Block)stat, identMap),
            Break => OnBreakStat((Break)stat, identMap),
            Case => OnCase((Case)stat, identMap),
            CompUnit => OnCompUnit((CompUnit)stat, identMap),
            Continue => OnContinueStat((Continue)stat, identMap),
            Default => OnDefault((Default)stat, identMap),
            DoLoop => OnDoLoopStat((DoLoop)stat, identMap),
            Expression => OnExpressionStat((Expression)stat, identMap),
            ForLoop => OnForLoopStat((ForLoop)stat, identMap),
            FunctionDecl => OnFunctionDecl((FunctionDecl)stat, identMap),
            Goto => OnGotoStat((Goto)stat, identMap),
            IfElse => OnIfElseStat((IfElse)stat, identMap),
            LabeledBlock => OnLabeledStatementStat((LabeledBlock)stat, identMap),
            NullStatement => OnNullStatementExpr((NullStatement)stat, identMap),
            ParamList => OnParamListStat((ParamList)stat, identMap),
            PostfixOp => OnPostfixOpStat((PostfixOp)stat, identMap),
            PrefixOp => OnPrefixOpStat((PrefixOp)stat, identMap),
            Return => OnReturnStat((Return)stat, identMap),
            Switch => OnSwitch((Switch)stat, identMap),
            Ternary => OnTernaryStat((Ternary)stat, identMap),
            VarDecl => OnVarDeclStat((VarDecl)stat, identMap),
            WhileLoop => OnWhileLoopStat((WhileLoop)stat, identMap),
            _ => OnStatDefault(stat, identMap)
        };
    }

    protected AstNode ResolveExpr(AstNode expr, IdentifierMap identMap)
    {
        return expr switch
        {
            Assignment => OnAssignmentExpr((Assignment)expr, identMap),
            BinaryOp => OnBinaryOpExpr((BinaryOp)expr, identMap),
            Constant => OnConstantExpr((Constant)expr, identMap),
            FunctionCall => OnFunctionCallExpr((FunctionCall)expr, identMap),
            NullStatement => OnNullStatementExpr((NullStatement)expr, identMap),
            PostfixOp => OnPostfixOpExpr((PostfixOp)expr, identMap),
            PrefixOp => OnPrefixOpExpr((PrefixOp)expr, identMap),
            Ternary => OnTernaryExpr((Ternary)expr, identMap),
            UnaryOp => OnUnaryOpExpr((UnaryOp)expr, identMap),
            Var => OnVarExpr((Var)expr, identMap),
            _ => OnExprDefault(expr, identMap),
        };
    }
}