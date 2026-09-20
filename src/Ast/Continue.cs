using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Continue() : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ContinueKw);

    public new static AstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ContinueKw);
        return new Continue();
    }

    public override string ToPrettyString(int indent = 0) => $"Continue()";

    public override IEnumerable<AstNode> Children() => [];
}