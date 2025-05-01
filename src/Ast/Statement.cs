using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Statement : AstNode
{
    public new static Statement Parse(Queue<Token> tokenStream)
    {
        var r = Return.Parse(tokenStream);
        Expect(Semicolon, tokenStream);
        return r;
    }
}