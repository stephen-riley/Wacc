using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class BlockItem : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => Statement.CanParse(tokenStream);

    public static Function Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IntKw);
        var ident = tokenStream.Expect(Identifier);
        tokenStream.Expect(OpenParen);
        tokenStream.Expect(VoidKw);
        tokenStream.Expect(CloseParen);
        tokenStream.Expect(OpenBrace);
        var stat = Statement.Parse(tokenStream);
        tokenStream.Expect(CloseBrace);

        return new Function("int", ident.Str ?? "", stat);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Function(");
        // sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"name={Name}");
        // sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"Body={Body.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}