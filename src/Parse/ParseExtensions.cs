using System.Diagnostics.CodeAnalysis;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Parse;

public static class ParseExtentions
{
    public static Token Expect(this Queue<Token> tokenStream, TokenType[] tokenTypes)
        => TryExpect(tokenTypes, tokenStream, out var topToken)
            ? topToken
            : throw new ParseError($"Expected {string.Join('/', tokenTypes)}, saw <{topToken?.TokenType}>");

    public static Token Expect(this Queue<Token> tokenStream, TokenType tokenType)
        => TryExpect([tokenType], tokenStream, out var topToken)
            ? topToken
            : throw new ParseError($"Expected {tokenType}, saw <{topToken?.TokenType}>");

    public static bool TryExpect(TokenType[] tokenTypes, Queue<Token> tokenStream, [NotNullWhen(true)] out Token? topToken)
    {
        var tok = tokenStream.Dequeue();

        foreach (var t in tokenTypes)
        {
            if (tok.TokenType == t)
            {
                topToken = tok;
                return true;
            }
        }

        topToken = tok;
        return false;
    }
}