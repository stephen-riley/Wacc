using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Label(string Name) : BlockItem
{
    public new static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(Identifier) && tokenStream.PeekFor(Colon, depth: 2);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var label = tokenStream.Expect(Identifier);
        tokenStream.Expect(Colon);
        return new Label(label.Str ?? throw new ParseError($"No string for label token {label}"));
    }

    public override string ToPrettyString(int indent = 0) => $"Label({Name})";
}