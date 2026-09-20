using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Break() : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(BreakKw);

    public new static AstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(BreakKw);
        return new Break();
    }

    public override string ToPrettyString(int indent = 0) => $"Break()";

    public override IEnumerable<AstNode> Children() => [];
}