using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Block(IAstNode[] BlockItems) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(OpenBrace)
        || BlockItem.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream, bool isDependent = false)
    {
        var children = new List<IAstNode>();

        if (tokenStream.PeekFor(OpenBrace))
        {
            tokenStream.Expect(OpenBrace);

            while (!tokenStream.PeekFor(CloseBrace))
            {
                children.Add(Parse(tokenStream));
            }

            tokenStream.Expect(CloseBrace);

            return new Block([.. children]);
        }
        else
        {
            return BlockItem.Parse(tokenStream, isDependent);
        }
    }

    public IEnumerable<IAstNode> Children() => BlockItems;

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Block(");

        foreach (var s in BlockItems)
        {
            sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(s.ToPrettyString(indent + 1));
        }

        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}