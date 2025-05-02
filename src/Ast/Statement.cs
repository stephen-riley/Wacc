using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class Statement : IAstNode
{
    public static Statement Parse(Queue<Token> tokenStream)
    {
        return tokenStream.Peek().TokenType switch
        {
            ReturnKw => Return.Parse(tokenStream),
            _ => throw new NotImplementedException()
        };
    }
}