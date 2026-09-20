using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public record OpAssignment(TokenType Operation, AstNode LExpr, AstNode RExpr) : AstNode
{
    public override bool IsBlockItem() => true;

    public new bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"OpAssignment(");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"op={Operation}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine(LExpr.ToPrettyString(indent + 1));
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine(RExpr.ToPrettyString(indent + 1));
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [LExpr, RExpr];
}