using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Function(string Type, string Name, Statement Body) : AstNode
{
    public new static Function Parse(Queue<Token> tokenStream)
    {
        Expect([IntKw], tokenStream);
        var ident = Expect([Identifier], tokenStream);
        Expect([OpenParen], tokenStream);
        Expect([VoidKw], tokenStream);
        Expect([CloseParen], tokenStream);
        Expect([OpenBrace], tokenStream);
        var stat = Statement.Parse(tokenStream);
        Expect([CloseBrace], tokenStream);

        return new Function("int", ident.Str ?? "", stat);
    }
}