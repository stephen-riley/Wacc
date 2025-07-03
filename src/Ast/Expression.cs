using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Expression(IAstNode SubExpr) : BlockItem
{
    public new static bool CanParse(Queue<Token> tokenStream)
        => Factor.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream, int minPrecedence = 0)
    {
        var left = Factor.Parse(tokenStream);

        var nextToken = tokenStream.Peek();
        while (nextToken.Is(BinaryOp.Operators) && BinaryOp.Precedence[nextToken.TokenType] >= minPrecedence)
        {
            if (BinaryOp.RightAssociativeOps.Contains(tokenStream.Peek().TokenType))
            {
                // TODO: there is a possible set equivalence issue between RighAssociativeOps and AssignOpMap
                if (tokenStream.TryExpect(Assignment.AssignOpMap.Keys, out var opAssignToken))
                {
                    var assignType = opAssignToken.TokenType;
                    var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType]);
                    right = new BinaryOp("*********", left, right);
                    left = new Assignment(left, right);
                }
                else
                {
                    tokenStream.Expect(Assign);
                    var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType]);
                    left = new Assignment(left, right);
                }
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

    public override string ToPrettyString(int indent = 0) => SubExpr.ToPrettyString(indent);
}