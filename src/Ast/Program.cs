using Wacc.Tokens;

namespace Wacc.Ast;

public record Program(Function Function) : AstNode
{
    public new static AstNode Parse(Queue<Token> tokenStream)
    {
        return new Program(Function.Parse(tokenStream));
    }
}