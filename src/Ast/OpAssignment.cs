using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public record OpAssignment(TokenType Operation, IAstNode LExpr, IAstNode RExpr) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"OpAssignment(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"op={Operation}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(LExpr.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(RExpr.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [LExpr, RExpr];
}