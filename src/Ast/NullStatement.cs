using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

// TODO: Not sure I like how this is handled in chapter 5...
public record NullStatement() : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(Semicolon);

    public static NullStatement Parse(Queue<Token> tokenStream) => new();

    public string ToPrettyString(int indent = 0) => "NullStatement()";

    public IEnumerable<IAstNode> Children() => [];
}