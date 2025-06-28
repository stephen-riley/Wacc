using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Expression(IAstNode SubExpr) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => Factor.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream, int minPrecedence = 0)
    {
        var left = Factor.Parse(tokenStream);

        var nextToken = tokenStream.Peek();
        while (nextToken.Is(BinaryOp.Operators) && BinaryOp.Precedence[nextToken.TokenType] >= minPrecedence)
        {
            if (BinaryOp.RightAssociativeOps.Contains(tokenStream.Peek().TokenType))
            {
                tokenStream.Expect(TokenType.Assign);
                var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType] + 1);
                left = new Assignment(left, right);
            }
            else if (tokenStream.TryExpect(BinaryOp.Operators, out var opToken))
            {
                var op = opToken.Str ?? throw new InvalidOperationException($"can't be {opToken}");
                var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType] + 1);
                left = new BinaryOp(op, left, right);
            }
            nextToken = tokenStream.Peek();
        }

        return left;
    }

    public string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");
}