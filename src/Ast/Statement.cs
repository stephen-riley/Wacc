using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class Statement : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(TokenType.ReturnKw);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        return tokenStream.Peek().TokenType switch
        {
            ReturnKw => Return.Parse(tokenStream),
            _ => throw new NotImplementedException($"{nameof(Statement)}.{nameof(Parse)}")
        };
    }

    public string ToPrettyString(int indent = 0)
        => throw new NotImplementedException($"{GetType().Name}.{nameof(ToPrettyString)}");
}