using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Continue(string Label) : IAstNode
{
    public const string DefaultLabel = "$__TODO_WHILE_LABEL__";

    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ContinueKw);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ContinueKw);
        return new Continue(DefaultLabel);
    }

    public string ToPrettyString(int indent = 0) => $"Continue({Label})";

    public IEnumerable<IAstNode> Children() => [];
}