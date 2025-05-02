using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class Return(Expression Expr) : Statement
{
    public new static Return Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ReturnKw);
        var expr = new Return(Expression.Parse(tokenStream));
        tokenStream.Expect(Semicolon);
        return expr;
    }
}