using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class Declaration(string identifier, IAstNode? expr = null) : IAstNode
{
    public string Identifier { get; internal set; } = identifier;
    public IAstNode? Expr { get; internal set; } = expr;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(IntKw);

    public static Declaration Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IntKw);
        var ident = tokenStream.Expect(TokenType.Identifier).Str ?? throw new ParseError("no identifier");
        var assignExpr = default(IAstNode);

        if (!tokenStream.PeekFor(Semicolon))
        {
            tokenStream.Expect(Assign);
            assignExpr = Expression.Parse(tokenStream);
        }
        tokenStream.Expect(Semicolon);

        return new Declaration(ident, assignExpr);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append("Declaration(\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append($"type={"int"}").Append('\n');
        sb.Append(IAstNode.IndentStr(indent + 1)).Append($"name={Identifier}").Append('\n');

        if (Expr is not null)
        {
            sb.Append(IAstNode.IndentStr(indent + 1)).Append($"Expr={Expr.ToPrettyString(indent + 1)}").Append('\n');
        }

        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}