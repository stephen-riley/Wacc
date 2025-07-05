using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public record PostfixOp(TokenType Op, IAstNode LValue) : BlockItem
{
    public new static bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"PostfixOp({Op.Description()}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(LValue.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}