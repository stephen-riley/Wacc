using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Default(IAstNode DefaultBlock) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(DefaultKw);

    public static Default Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(DefaultKw);
        tokenStream.Expect(Colon);
        var defaultBlock = Block.Parse(tokenStream);

        return new Default(defaultBlock);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Default(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"block={DefaultBlock.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [DefaultBlock];
}