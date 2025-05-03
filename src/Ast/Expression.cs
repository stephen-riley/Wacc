using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Expression : IAstNode
{
    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        return tokenStream.Peek().TokenType switch
        {
            TokenType.Constant => Constant.Parse(tokenStream),
            _ => throw new NotImplementedException($"")
        };
    }

    public string ToPrettyString(int indent = 0)
        => throw new NotImplementedException($"{nameof(Expression)}.{nameof(ToPrettyString)}");
}