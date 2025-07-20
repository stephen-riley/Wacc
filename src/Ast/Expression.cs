using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

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
                // TODO: there is a possible set equivalence issue between RighAssociativeOps and AssignOpMap
                if (nextToken.Is([.. Assignment.AssignOpMap.Keys]))
                {
                    var assignType = nextToken.TokenType;
                    tokenStream.Expect(Assignment.AssignOpMap.Keys);
                    var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType]);
                    right = new BinaryOp(Assignment.AssignOpMap[assignType], left, right);
                    left = new Assignment(left, right);
                }
                else if (nextToken.Is(Assign))
                {
                    tokenStream.Expect(Assign);
                    var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType]);
                    left = new Assignment(left, right);
                }
                else if (nextToken.Is(Question))
                {
                    // At this point, `left` is the condition.
                    left = Ternary.Parse(tokenStream, left);
                }
                else
                {
                    throw new NotImplementedException($"can't yet handle right-associative operator {nextToken}");
                }
            }
            else // left-associative operator
            {
                if (tokenStream.TryExpect(BinaryOp.Operators, out var opToken))
                {
                    if (opToken.TokenType is Increment or Decrement)
                    {
                        left = new PostfixOp(nextToken.TokenType, left);
                    }
                    else
                    {
                        var right = Parse(tokenStream, BinaryOp.Precedence[nextToken.TokenType] + 1);
                        left = new BinaryOp(opToken.TokenType, left, right);

                    }
                }
            }
            nextToken = tokenStream.Peek();
        }

        return left;
    }

    public IEnumerable<IAstNode> Children() => [];

    public string ToPrettyString(int indent = 0) => SubExpr.ToPrettyString(indent);
}