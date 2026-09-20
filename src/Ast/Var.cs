using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Var(string Name) : AstNode
{
    public new bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(TokenType.Identifier);

    public new static Var Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Expect(TokenType.Identifier);
        return new Var(tok.Str ?? throw new ParseError($"no Str for token {tok}"));
    }

    public override string ToPrettyString(int indent = 0) => $"Var({Name})";

    public override IEnumerable<AstNode> Children() => [];
}