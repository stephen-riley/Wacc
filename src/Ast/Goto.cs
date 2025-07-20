using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Goto(Label Label) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(GotoKw) && tokenStream.PeekFor(Identifier, depth: 2);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(GotoKw);
        var label = tokenStream.Expect(Identifier);
        if (label.Str is null)
        {
            throw new ParseError($"Invalid label token {label}");
        }

        return new Goto(new Label(label.Str));
    }

    public string ToPrettyString(int indent = 0) => $"Goto({Label.ToPrettyString()})";

    public IEnumerable<IAstNode> Children() => [];
}