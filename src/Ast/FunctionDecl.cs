using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record FunctionDecl(string FuncType, Var Identifier, ParamList Params, Block? Body = null) : AstNode
{
    public IdentifierMap? VariableMap;

    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekForSequence([IntKw, TokenType.Identifier, OpenParen]);

    public new static FunctionDecl Parse(Queue<Token> tokenStream)
    {
        var leadToken = tokenStream.Peek();

        tokenStream.Expect(IntKw);
        var ident = Var.Parse(tokenStream);
        tokenStream.Expect(OpenParen);
        var paramList = ParamList.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        if (tokenStream.PeekFor(Semicolon))
        {
            tokenStream.Expect(Semicolon);
            return new FunctionDecl("int", ident, paramList) { LeadToken = leadToken };
        }
        else
        {
            var body = Block.Parse(tokenStream);
            return new FunctionDecl("int", ident, paramList, (Block)body) { LeadToken = leadToken };
        }
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append("FunctionDecl(\n");
        sb.Append(IndentStr(indent + 1)).Append($"type={FuncType}").Append('\n');
        sb.Append(IndentStr(indent + 1)).Append(Identifier.ToPrettyString()).Append('\n');
        sb.Append(IndentStr(indent + 1)).Append(Params.ToPrettyString()).Append('\n');

        if (Body is not null)
        {
            sb.Append(IndentStr(indent + 1)).Append(Body.ToPrettyString()).Append('\n');
        }
        sb.Append(IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => Body is not null ? [Params, Body] : [Params];
}