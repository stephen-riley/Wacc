using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Factor(IAstNode SubExpr) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor([TokenType.Constant, Identifier, OpenParen])
            || UnaryOp.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Peek();
        var factor = tok.TokenType switch
        {
            TokenType.Constant => Constant.Parse(tokenStream),
            Identifier => Var.Parse(tokenStream),
            OpenParen => Ext.Do(() =>
            {
                tokenStream.Expect(OpenParen);
                var expr = Expression.Parse(tokenStream);
                tokenStream.Expect(CloseParen);
                return expr;
            }),
            _ => UnaryOp.Parse(tokenStream)
        };

        if (tokenStream.PeekFor([Increment, Decrement]))
        {
            var op = tokenStream.Expect(tokenStream.Peek().TokenType);
            factor = op.TokenType switch
            {
                Increment => new PostfixOp(Increment, factor),
                Decrement => new PostfixOp(Decrement, factor),
                _ => factor
            };
        }

        return factor;
    }

    public string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");

    public IEnumerable<IAstNode> Children() => [];
}