using System.Text;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record CompUnit(Function[] Functions) : AstNode
{
    public new static CompUnit Parse(Queue<Token> tokenStream)
    {
        var stats = new List<Function>();

        while (!tokenStream.PeekFor(EOF))
        {
            stats.Add(Function.Parse(tokenStream));
        }

        return new CompUnit([.. stats]);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append("Program(\n");
        foreach (var stat in Functions)
        {
            sb.Append(IndentStr(indent + 1));
            sb.Append(stat.ToPrettyString(indent + 1)).Append('\n');
        }
        sb.Append(AstNode.INDENT.X(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [.. Functions];
}