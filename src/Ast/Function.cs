using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Function(string Type, string Name, IAstNode[] Body) : IAstNode
{
    public static Function Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IntKw);
        var ident = Var.Parse(tokenStream);
        tokenStream.Expect(OpenParen);
        tokenStream.Expect(VoidKw);
        tokenStream.Expect(CloseParen);
        tokenStream.Expect(OpenBrace);

        var body = new List<IAstNode>();
        while (tokenStream.Peek().TokenType != CloseBrace)
        {
            body.Add(BlockItem.Parse(tokenStream));
        }
        tokenStream.Expect(CloseBrace);

        return new Function("int", ident.Name ?? "", [.. body]);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Function(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"name={Name}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"Body=(");
        foreach (var s in Body)
        {
            sb.Append(IAstNode.IndentStr(indent + 2)).AppendLine(s.ToPrettyString(indent + 2));
        }
        sb.AppendLine(IAstNode.IndentStr(indent + 1)).Append(')');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}