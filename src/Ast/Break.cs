using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Break : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(BreakKw);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(BreakKw);
        return new Break();
    }

    public string ToPrettyString(int indent = 0) => $"Break()";

    public IEnumerable<IAstNode> Children() => [];
}