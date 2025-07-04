using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Statement : BlockItem
{
    public new static bool CanParse(Queue<Token> tokenStream)
        => NullStatement.CanParse(tokenStream)
            || Return.CanParse(tokenStream)
            || Expression.CanParse(tokenStream);

    public new static IAstNode Parse(Queue<Token> tokenStream)
    {
        var stat = tokenStream.Peek().TokenType switch
        {
            Semicolon => NullStatement.Parse(tokenStream),
            ReturnKw => Return.Parse(tokenStream),
            _ => Expression.Parse(tokenStream)
        };

        tokenStream.Expect(Semicolon);
        return stat;
    }

    public override string ToPrettyString(int indent = 0)
        => throw new NotImplementedException($"{GetType().Name}.{nameof(ToPrettyString)}");
}