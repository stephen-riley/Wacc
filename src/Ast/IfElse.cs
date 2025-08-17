using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record IfElse(IAstNode CondExpr, IAstNode ThenBlock, IAstNode? ElseBlock) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(IfKw);

    public static IfElse Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IfKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var thenNode = Block.Parse(tokenStream, isDependent: true);
        var thenBlock = thenNode is Block || thenNode.IsBlockItem() ? thenNode : throw new ParseError($"{thenNode} is not a Block or BlockItem");

        IAstNode? elseBlock = null;

        if (tokenStream.PeekFor(ElseKw))
        {
            tokenStream.Expect(ElseKw);
            var elseNode = Block.Parse(tokenStream, isDependent: true);
            elseBlock = elseNode is Block || elseNode.IsBlockItem() ? elseNode : throw new ParseError($"{elseNode} is not a Block or BlockItem");
        }

        return new IfElse(condExpr, thenBlock, elseBlock);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("IfElse(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"condition={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"then={ThenBlock.ToPrettyString(indent + 1)}");
        if (ElseBlock is not null)
        {
            sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"else={CondExpr.ToPrettyString(indent + 1)}");
        }
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => ElseBlock is not null ? [CondExpr, ThenBlock, ElseBlock] : [CondExpr, ThenBlock];
}