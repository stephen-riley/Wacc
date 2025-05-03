using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Constant(int Int) : IAstNode
{
    public static Constant Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Expect(TokenType.Constant);
        return new Constant(tok.Int);
    }

    public string ToPrettyString(int indent = 0) => $"Constant({Int})";
}