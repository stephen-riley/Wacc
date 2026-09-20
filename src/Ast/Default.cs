using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record Default(AstNode DefaultBlock) : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(DefaultKw);

    public new static Default Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(DefaultKw);
        tokenStream.Expect(Colon);
        var defaultBlock = Block.Parse(tokenStream);

        return new Default(defaultBlock);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Default(");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"block={DefaultBlock.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [DefaultBlock];
}