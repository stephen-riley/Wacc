using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Goto(string LabelName) : BlockItem
{
    public new static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(GotoKw) && tokenStream.PeekFor(Identifier, depth: 2);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(GotoKw);
        var label = tokenStream.Expect(Identifier);
        return new Goto(label.Str ?? throw new ParseError($"No string for label token {label}"));
    }

    public override string ToPrettyString(int indent = 0) => $"Goto({LabelName})";
}