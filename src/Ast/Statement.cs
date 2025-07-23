using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Statement() : IAstNode
{
    // This will list the statement types that don't have semicolons after them.
    public static readonly HashSet<TokenType> BlockStatements = [IfKw];

    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream)
        => NullStatement.CanParse(tokenStream)
            || Return.CanParse(tokenStream)
            || IfElse.CanParse(tokenStream)
            || Goto.CanParse(tokenStream)
            || Expression.CanParse(tokenStream)
            || Label.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream, bool nested = false)
    {
        if (Label.CanParse(tokenStream))
        {
            return LabeledStatement.Parse(tokenStream);
        }

        var nextTokenType = tokenStream.Peek().TokenType;

        var stat = nextTokenType switch
        {
            Semicolon => NullStatement.Parse(tokenStream),
            ReturnKw => Return.Parse(tokenStream),
            IfKw => IfElse.Parse(tokenStream),
            GotoKw => Goto.Parse(tokenStream),
            _ => Expression.Parse(tokenStream)
        };

        if (!nested && !BlockStatements.Contains(nextTokenType))
        {
            tokenStream.Expect(Semicolon);
        }

        return stat;
    }

    public string ToPrettyString(int indent = 0)
        => throw new NotImplementedException($"{GetType().Name}.{nameof(ToPrettyString)}");

    public IEnumerable<IAstNode> Children() => [];
}