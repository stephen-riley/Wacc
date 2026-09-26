using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record PrefixOp(TokenType Op, AstNode LValExpr) : AstNode
{
    public new bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"PrefixOp({Op.Description()}");
        sb.Append(IndentStr(indent + 1)).AppendLine(LValExpr.ToPrettyString(indent + 1));
        sb.Append(IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [LValExpr];
}