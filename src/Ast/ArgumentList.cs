using System.Text;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record ArgumentList(IEnumerable<AstNode> Params) : AstNode
{
    public override bool IsBlockItem() => false;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekForOneOf([Identifier]);

    public bool IsVoid() => !Params.Any();

    public new static ArgumentList Parse(Queue<Token> tokenStream)
    {
        var arguments = new List<AstNode>();
        if (!tokenStream.PeekFor(VoidKw))
        {
            tokenStream.Expect(IntKw);
            arguments.Add(Expression.Parse(tokenStream));

            while (tokenStream.PeekFor(Comma))
            {
                tokenStream.Expect(Comma);
                tokenStream.Expect(IntKw);
                arguments.Add(Var.Parse(tokenStream));
            }
        }
        else
        {
            tokenStream.Expect(VoidKw);
        }

        return new ArgumentList(arguments);
    }

    public override string ToPrettyString(int indent = 0)
    {
        if (IsVoid())
        {
            return "ArgumentList(void)";
        }

        var sb = new StringBuilder();
        sb.Append("ArgumentList(\n");
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