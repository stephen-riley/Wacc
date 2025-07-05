using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record UnaryOp(Token Op, IAstNode Expr) : IAstNode
{
    internal static readonly List<TokenType> UnaryOpTokens = [Complement, MinusSign, LogicalNot, Increment, Decrement];

    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(UnaryOpTokens);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var op = tokenStream.Expect(UnaryOpTokens);
        var expr = Factor.Parse(tokenStream);
        return op.TokenType switch
        {
            Increment or Decrement => new PrefixOp(op.TokenType, expr),
            _ => new UnaryOp(op, expr)
        };
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"Unary('{Op}'\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(Expr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}