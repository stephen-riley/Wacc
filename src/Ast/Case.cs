using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Case(IAstNode CaseCondExpr, IAstNode CaseBlock) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(CaseKw);

    public static Case Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(CaseKw);
        var caseCond = Constant.Parse(tokenStream);
        tokenStream.Expect(Colon);
        var caseBlock = Block.Parse(tokenStream);

        return new Case(caseCond, caseBlock);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Case(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"expr={CaseCondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"block={CaseBlock.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [CaseCondExpr, CaseBlock];
}