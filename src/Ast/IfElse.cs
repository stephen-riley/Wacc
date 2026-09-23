using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record IfElse(AstNode CondExpr, AstNode ThenBlock, AstNode? ElseBlock) : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(IfKw);

    public new static IfElse Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IfKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var thenNode = Block.Parse(tokenStream, isDependent: true);
        var thenBlock = thenNode is Block || thenNode.IsBlockItem() ? thenNode : throw new ParseError($"{thenNode} is not a Block or BlockItem");

        AstNode? elseBlock = null;

        if (tokenStream.PeekFor(ElseKw))
        {
            tokenStream.Expect(ElseKw);
            var elseNode = Block.Parse(tokenStream, isDependent: true);
            elseBlock = elseNode is Block || elseNode.IsBlockItem() ? elseNode : throw new ParseError($"{elseNode} is not a Block or BlockItem");
        }

        return new IfElse(condExpr, thenBlock, elseBlock);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("IfElse(");
        sb.Append(IndentStr(indent + 1)).AppendLine($"condition={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IndentStr(indent + 1)).AppendLine($"then={ThenBlock.ToPrettyString(indent + 1)}");
        if (ElseBlock is not null)
        {
            sb.Append(IndentStr(indent + 1)).AppendLine($"else={ElseBlock.ToPrettyString(indent + 1)}");
        }
        sb.Append(IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => ElseBlock is not null ? [CondExpr, ThenBlock, ElseBlock] : [CondExpr, ThenBlock];
}