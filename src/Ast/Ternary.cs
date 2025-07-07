using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Ternary(IAstNode CondExpr, IAstNode Middle, IAstNode Right) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(Question);

    public static IAstNode Parse(Queue<Token> tokenStream, IAstNode left)
    {
        // At this point, `left` contains the condition expression.
        // .
        // Get the consequent term:
        tokenStream.Expect(Question);
        var middle = Expression.Parse(tokenStream, 0);

        // Get the alternative term:
        tokenStream.Expect(Colon);
        var right = Expression.Parse(tokenStream, BinaryOp.Precedence[Question]);

        return new Ternary(left, middle, right);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Ternary(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"condition={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"middle={Middle.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"right={Right.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();

    }
}