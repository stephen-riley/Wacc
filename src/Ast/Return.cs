using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Return(IAstNode Expr) : BlockItem
{
    public static new bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ReturnKw);

    public static Return Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ReturnKw);
        var retExpr = Expression.Parse(tokenStream);
        var expr = new Return(retExpr);
        return expr;
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Return(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(Expr.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}