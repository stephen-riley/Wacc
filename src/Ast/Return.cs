using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Return(IAstNode expr) : BlockItem
{
    public IAstNode Expr { get; internal set; } = expr;

    public static new bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ReturnKw);

    public static new Return Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ReturnKw);
        var expr = new Return(Expression.Parse(tokenStream));
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