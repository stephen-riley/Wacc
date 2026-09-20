using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record Block(AstNode[] BlockItems) : AstNode
{
    public VarMap? VariableMap;

    public new bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor(OpenBrace)
        || BlockItem.CanParse(tokenStream);

    public static AstNode Parse(Queue<Token> tokenStream, bool isDependent = false, bool forceBlock = false)
    {
        var children = new List<AstNode>();

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
        else if (!forceBlock)
        {
            return BlockItem.Parse(tokenStream, isDependent);
        }
        else
        {
            throw new ParseError("caller forced a full block but a statement was found");
        }
    }

    public override IEnumerable<AstNode> Children() => BlockItems;

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Block(");

        foreach (var s in BlockItems)
        {
            sb.Append(AstNode.IndentStr(indent + 1)).AppendLine(s.ToPrettyString(indent + 1));
        }

        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}