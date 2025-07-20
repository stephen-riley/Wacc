using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Constant(int Int) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.Peek().TokenType == TokenType.Constant;

    public static Constant Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Expect(TokenType.Constant);
        return new Constant(tok.Int);
    }

    public IEnumerable<IAstNode> Children() => [];

    public string ToPrettyString(int indent = 0) => $"Constant({Int})";
}