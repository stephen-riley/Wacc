using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Switch(IAstNode CondExpr, IAstNode CaseBlock) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(SwitchKw);

    public static Switch Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(SwitchKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var caseBlock = Block.Parse(tokenStream);

        return new Switch(condExpr, caseBlock);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Switch(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"condition={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"block={CaseBlock.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [CondExpr, CaseBlock];
}