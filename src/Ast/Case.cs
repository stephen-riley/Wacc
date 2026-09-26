using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record Case(AstNode CaseCondExpr) : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(CaseKw);

    public new static Case Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(CaseKw);
        var caseCond = Constant.Parse(tokenStream);
        tokenStream.Expect(Colon);

        return new Case(caseCond);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Case(");
        sb.Append(IndentStr(indent + 1)).AppendLine($"expr={CaseCondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [CaseCondExpr /*, CaseBlock */];
}