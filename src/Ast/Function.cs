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
            var item = BlockItem.Parse(tokenStream);
            body.Add(item);
        }

        tokenStream.Expect(CloseBrace);

        return new Function("int", ident.Name ?? "", [.. body]);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Function(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"name={Name}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"body=[");

        foreach (var s in Body)
        {
            sb.Append(IAstNode.IndentStr(indent + 2)).AppendLine(s.ToPrettyString(indent + 2));
        }

        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine("]");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [.. Body];
}