using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Function(string Type, string Name, Statement Body) : IAstNode
{
    public static Function Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(IntKw);
        var ident = tokenStream.Expect(Identifier);
        tokenStream.Expect(OpenParen);
        tokenStream.Expect(VoidKw);
        tokenStream.Expect(CloseParen);
        tokenStream.Expect(OpenBrace);
        var stat = Statement.Parse(tokenStream);
        tokenStream.Expect(CloseBrace);

        return new Function("int", ident.Str ?? "", stat);
    }
}