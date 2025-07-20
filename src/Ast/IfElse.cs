using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record IfElse(IAstNode CondExpr, IAstNode ThenStat, IAstNode? ElseStat) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(IfKw);

    public static IfElse Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IfKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var thenNode = BlockItem.Parse(tokenStream, isDependent: true);
        var thenStat = thenNode.IsBlockItem() ? thenNode : throw new ParseError($"{thenNode} is not a BlockItem");

        IAstNode? elseStat = null;

        if (tokenStream.PeekFor(ElseKw))
        {
            tokenStream.Expect(ElseKw);
            var elseNode = BlockItem.Parse(tokenStream, isDependent: true);
            elseStat = elseNode.IsBlockItem() ? elseNode : throw new ParseError($"{elseNode} is not a BlockItem");
        }

        return new IfElse(condExpr, thenStat, elseStat);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("IfElse(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"condition={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"then={ThenStat.ToPrettyString(indent + 1)}");
        if (ElseStat is not null)
        {
            sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"else={CondExpr.ToPrettyString(indent + 1)}");
        }
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => ElseStat is not null ? [CondExpr, ThenStat, ElseStat] : [CondExpr, ThenStat];
}