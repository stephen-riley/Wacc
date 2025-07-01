using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Declaration(string DeclType, Var Identifier, IAstNode? Expr = null) : BlockItem
{
    // public string DeclType => declType;
    // public Var Identifier => identifier;
    // public IAstNode? Expr => expr;

    // public void Deconstruct(out string declType, out Var identifier, out IAstNode? expr)
    // {
    //     declType = DeclType;
    //     identifier = Identifier;
    //     expr = Expr;
    // }

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(IntKw);

    public new static Declaration Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IntKw);
        var ident = Var.Parse(tokenStream);
        var assignExpr = default(IAstNode);

        if (!tokenStream.PeekFor(Semicolon))
        {
            tokenStream.Expect(Assign);
            assignExpr = Expression.Parse(tokenStream);
        }
        tokenStream.Expect(Semicolon);

        return new Declaration("int", ident, assignExpr);
    }

    public override string ToPrettyString(int indent = 0)
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
}