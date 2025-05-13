using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public class UnaryOp(string op, IAstNode expr) : IAstNode
{
    public IAstNode Expr => expr;
    public string Op => op;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.Peek().TokenType is TokenType.Complement or TokenType.MinusSign;

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var op = tokenStream.Expect([TokenType.Complement, TokenType.MinusSign]).Str;
        var expr = Expression.Parse(tokenStream);
        return new UnaryOp(op ?? throw new ParseError($"can't extract operator from {op}"), expr);
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