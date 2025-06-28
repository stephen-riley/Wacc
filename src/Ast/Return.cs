using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class Return(IAstNode expr) : IAstNode
{
    public IAstNode Expr { get; internal set; } = expr;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ReturnKw);

    public static Return Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ReturnKw);
        var expr = new Return(Expression.Parse(tokenStream));
        return expr;
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append("Return(\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(Expr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}