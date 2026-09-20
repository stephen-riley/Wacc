using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record Constant(int Int) : AstNode
{
    public new bool CanParse(Queue<Token> tokenStream) => tokenStream.Peek().TokenType == TokenType.Constant;

    public new static Constant Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Expect(TokenType.Constant);
        return new Constant(tok.Int);
    }

    public override IEnumerable<AstNode> Children() => [];

    public override string ToPrettyString(int indent = 0) => $"Constant({Int})";
}