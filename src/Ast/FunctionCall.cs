using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record FunctionCall(Var Identifier, ArgumentList Args) : AstNode
{
    public IdentifierMap? VariableMap;

    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekForSequence([TokenType.Identifier, OpenParen]);

    public new static FunctionCall Parse(Queue<Token> tokenStream)
    {
        var leadToken = tokenStream.Peek();

        var ident = Var.Parse(tokenStream);
        tokenStream.Expect(OpenParen);
        var args = ArgumentList.Parse(tokenStream);
        tokenStream.Expect(CloseParen);
        return new FunctionCall(ident, args) { LeadToken = leadToken };
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append("FunctionDecl(\n");
        sb.Append(IndentStr(indent + 1)).Append(Identifier.ToPrettyString()).Append('\n');
        sb.Append(IndentStr(indent + 1)).Append(Args.ToPrettyString()).Append('\n');
        sb.Append(IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [];
}