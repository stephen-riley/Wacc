using Wacc.Tokens;

namespace Wacc.Ast;

public record Program(Function Function) : IAstNode
{
    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        return new Program(Function.Parse(tokenStream));
    }
}