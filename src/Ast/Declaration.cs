using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record Declaration : AstNode
{
    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream)
        => FunctionDecl.CanParse(tokenStream)
        || VarDecl.CanParse(tokenStream);

    public new static AstNode Parse(Queue<Token> tokenStream)
    {
        var leadToken = tokenStream.Peek();

        if (FunctionDecl.CanParse(tokenStream))
        {
            return FunctionDecl.Parse(tokenStream) with { LeadToken = leadToken };
        }
        else
        {
            return VarDecl.Parse(tokenStream) with { LeadToken = leadToken };
        }
    }

    public override string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");

    public override IEnumerable<AstNode> Children() => [];
}