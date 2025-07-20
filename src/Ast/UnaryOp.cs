using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record UnaryOp(Token Op, IAstNode Expr) : IAstNode
{
    internal static readonly List<TokenType> UnaryOpTokens = [Complement, Minus, LogicalNot, Increment, Decrement];

    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(UnaryOpTokens);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var op = tokenStream.Expect(UnaryOpTokens);
        var factor = Factor.Parse(tokenStream);
        return op.TokenType switch
        {
            Increment or Decrement => new PrefixOp(op.TokenType, factor),
            _ => new UnaryOp(op, factor)
        };
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"Unary('{Op.TokenType}'\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(Expr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [Expr];
}