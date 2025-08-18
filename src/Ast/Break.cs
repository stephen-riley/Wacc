using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Break(string Label) : IAstNode
{
    public const string DefaultLabel = "$__TODO_BREAK_LABEL__";

    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(BreakKw);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(BreakKw);
        return new Break(DefaultLabel);
    }

    public string ToPrettyString(int indent = 0) => $"Break({Label})";

    public IEnumerable<IAstNode> Children() => [];
}