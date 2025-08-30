using System.Text;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record ForInit(string DeclType, Var Identifier, IAstNode? Expr = null) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => Declaration.CanParse(tokenStream) | Expression.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        // `Declaration` eats its terminal semicolon, so `ForInit` makes sure
        // to eat `Expressions`'s as well for consistency.

        if (tokenStream.PeekFor(TokenType.Semicolon))
        {
            tokenStream.Expect(TokenType.Semicolon);
            return new NullStatement();
        }
        else if (Declaration.CanParse(tokenStream))
        {
            return Declaration.Parse(tokenStream);
        }
        else
        {
            var expr = Expression.Parse(tokenStream);
            tokenStream.Expect(TokenType.Semicolon);
            return expr;
        }
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append("Declaration(\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append($"type={DeclType}").Append('\n');
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(Identifier.ToPrettyString()).Append('\n');

        if (Expr is not null)
        {
            sb.Append(IAstNode.IndentStr(indent + 1)).Append(Expr.ToPrettyString(indent + 1)).Append('\n');
        }

        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => Expr is not null ? [Expr] : [];
}