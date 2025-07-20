using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Var(string Name) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(TokenType.Identifier);

    public static Var Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Expect(TokenType.Identifier);
        return new Var(tok.Str ?? throw new ParseError($"no Str for token {tok}"));
    }

    public string ToPrettyString(int indent = 0) => $"Var({Name})";

    public IEnumerable<IAstNode> Children() => [];
}