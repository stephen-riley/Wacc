using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record Switch(AstNode CondExpr, AstNode CaseBlock) : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(SwitchKw);

    public new static Switch Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(SwitchKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var caseBlock = Block.Parse(tokenStream);

        return new Switch(condExpr, caseBlock);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Switch(");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"condition={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"block={CaseBlock.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [CondExpr, CaseBlock];
}