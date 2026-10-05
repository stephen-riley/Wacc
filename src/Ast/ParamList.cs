using System.Text;
using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record ParamList(IEnumerable<Var> Params) : AstNode
{
    public override bool IsBlockItem() => false;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekForOneOf([VoidKw, IntKw]);

    public bool IsVoid() => !Params.Any();

    public new static ParamList Parse(Queue<Token> tokenStream)
    {
        var leadToken = tokenStream.Peek();

        var paramList = new List<Var>();
        if (!tokenStream.PeekFor(VoidKw))
        {
            tokenStream.Expect(IntKw);
            paramList.Add(Var.Parse(tokenStream));

            while (tokenStream.PeekFor(Comma))
            {
                tokenStream.Expect(Comma);
                tokenStream.Expect(IntKw);
                paramList.Add(Var.Parse(tokenStream));
            }
        }
        else
        {
            tokenStream.Expect(VoidKw);
        }

        return new ParamList(paramList) { LeadToken = leadToken };
    }

    public override string ToPrettyString(int indent = 0)
    {
        if (IsVoid())
        {
            return "ParamList(void)";
        }

        var sb = new StringBuilder();
        sb.Append("ParamList(\n");
        foreach (var v in Params)
        {
            sb.Append(IndentStr(indent + 1));
            sb.Append(v.ToPrettyString(indent + 1)).Append('\n');
        }
        sb.Append(INDENT.X(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [];
}