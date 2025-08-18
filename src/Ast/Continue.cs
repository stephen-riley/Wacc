using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Continue : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ContinueKw);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ContinueKw);
        return new Continue();
    }

    public string ToPrettyString(int indent = 0) => $"Continue()";

    public IEnumerable<IAstNode> Children() => [];
}