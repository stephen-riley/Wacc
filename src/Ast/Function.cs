using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Function(string Type, string Name, Block Body) : IAstNode
{
    public static Function Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IntKw);
        var ident = Var.Parse(tokenStream);
        tokenStream.Expect(OpenParen);
        tokenStream.Expect(VoidKw);
        tokenStream.Expect(CloseParen);

        var block = (Block)Block.Parse(tokenStream);

        return new Function("int", ident.Name ?? "", block);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Function(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"name={Name}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"body=");

        sb.Append(IAstNode.IndentStr(indent + 2)).AppendLine(Body.ToPrettyString(indent + 2));

        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [Body];
}