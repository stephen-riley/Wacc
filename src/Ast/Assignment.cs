using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Assignment(IAstNode LExpr, IAstNode RExpr) : BlockItem
{
    public new static bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"Assignment(\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(LExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(RExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}