using System.Text;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Expression(IAstNode SubExpr) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor([TokenType.Constant, TokenType.OpenParen])
            || UnaryOp.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Peek();

        return tok.TokenType switch
        {
            TokenType.Constant => Constant.Parse(tokenStream),
            TokenType.OpenParen => Ext.Do(() =>
            {
                tokenStream.Expect(TokenType.OpenParen);
                var expr = Parse(tokenStream);
                tokenStream.Expect(TokenType.CloseParen);
                return expr;
            }),
            _ => UnaryOp.Parse(tokenStream)
        };
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Expression(");
        sb.Append(IAstNode.IndentStr(indent + 1));
        sb.AppendLine(SubExpr.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent));
        sb.Append(')');
        return sb.ToString();
    }
}