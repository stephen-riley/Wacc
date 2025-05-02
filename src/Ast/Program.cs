using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Program(IEnumerable<Function> Function) : IAstNode
{
    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var stats = new List<Function>();

        while (!tokenStream.PeekFor(EOF))
        {
            stats.Add(Ast.Function.Parse(tokenStream));
        }

        return new Program(stats);
    }
}