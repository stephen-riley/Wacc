using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Statement : BlockItem
{
    // This will list the statement types that don't have semicolons after them.
    public static readonly HashSet<TokenType> BlockStatements = [IfKw];

    public new static bool CanParse(Queue<Token> tokenStream)
        => NullStatement.CanParse(tokenStream)
            || Return.CanParse(tokenStream)
            || IfElse.CanParse(tokenStream)
            || Expression.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var nextTokenType = tokenStream.Peek().TokenType;

        var stat = nextTokenType switch
        {
            Semicolon => NullStatement.Parse(tokenStream),
            ReturnKw => Return.Parse(tokenStream),
            IfKw => IfElse.Parse(tokenStream),
            _ => Expression.Parse(tokenStream)
        };

        if (!BlockStatements.Contains(nextTokenType))
        {
            tokenStream.Expect(Semicolon);
        }

        return stat;
    }

    public override string ToPrettyString(int indent = 0)
        => throw new NotImplementedException($"{GetType().Name}.{nameof(ToPrettyString)}");
}