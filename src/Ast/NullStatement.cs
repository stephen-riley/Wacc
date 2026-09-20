using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

// TODO: Not sure I like how this is handled in chapter 5...
public partial record NullStatement() : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(Semicolon);

    public new static NullStatement Parse(Queue<Token> tokenStream) => new();

    public override string ToPrettyString(int indent = 0) => "NullStatement()";

    public override IEnumerable<AstNode> Children() => [];
}