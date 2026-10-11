using System.Text;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record CompUnit(FunctionDecl[] Functions) : AstNode
{
    public IdentifierMap? IdentifierMap;

    public new static CompUnit Parse(Queue<Token> tokenStream)
    {
        var leadToken = tokenStream.Peek();

        var stats = new List<FunctionDecl>();

        while (!tokenStream.PeekFor(EOF))
        {
            stats.Add(FunctionDecl.Parse(tokenStream));
        }

        return new CompUnit([.. stats]);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"{nameof(Program)}(\n");
        foreach (var stat in Functions)
        {
            sb.Append(IndentStr(indent + 1));
            sb.Append(stat.ToPrettyString(indent + 1)).Append('\n');
        }
        sb.Append(INDENT.X(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [.. Functions];
}