using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Return(Expression Expr) : Statement
{
    public new static Return Parse(Queue<Token> tokenStream)
    {
        Expect(ReturnKw, tokenStream);
        return new(Expression.Parse(tokenStream));
    }
}