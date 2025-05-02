using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Expression(int Int) : IAstNode
{
    public static Expression Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Expect(Constant);
        return new Expression(tok.Int);
    }
}