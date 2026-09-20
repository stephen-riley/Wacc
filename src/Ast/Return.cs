using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Return(AstNode Expr) : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ReturnKw);

    public new static Return Parse(Queue<Token> tokenStream)
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
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine(Expr.ToPrettyString(indent + 1));
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [Expr];
}