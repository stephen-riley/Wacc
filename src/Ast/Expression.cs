using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Expression(int Int) : AstNode
{
    public new static Expression Parse(Queue<Token> tokenStream)
    {
        var tok = Expect(Constant, tokenStream);
        return new Expression(tok.Int);
    }
}