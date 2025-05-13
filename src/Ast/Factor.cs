using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Ast;

public record Factor(IAstNode SubExpr) : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => tokenStream.PeekFor([TokenType.Constant, TokenType.OpenParen])
            || UnaryOp.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var tok = tokenStream.Peek();

        return tok.TokenType switch
        {
            TokenType.Constant => Constant.Parse(tokenStream),
            TokenType.OpenParen => Ext.Do(() =>
            {
                tokenStream.Expect(TokenType.OpenParen);
                var expr = Expression.Parse(tokenStream);
                tokenStream.Expect(TokenType.CloseParen);
                return expr;
            }),
            _ => UnaryOp.Parse(tokenStream)
        };
    }

    public string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");
}