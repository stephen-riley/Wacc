using System.Diagnostics.CodeAnalysis;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Parse;

public static class ParseExtentions
{
    public static Token Expect(this Queue<Token> tokenStream, TokenType[] tokenTypes)
        => tokenStream.TryExpect(tokenTypes, out var topToken)
            ? topToken
            : throw new ParseError($"Expected {string.Join('/', tokenTypes)}, saw <{topToken?.TokenType}>");

    public static Token Expect(this Queue<Token> tokenStream, TokenType tokenType)
        => tokenStream.TryExpect([tokenType], out var topToken)
            ? topToken
            : throw new ParseError($"Expected {tokenType}, saw <{topToken?.TokenType}>");

    public static bool TryExpect(this Queue<Token> tokenStream, TokenType[] tokenTypes, [NotNullWhen(true)] out Token? topToken)
    {
        topToken = tokenStream.Dequeue();
        return new HashSet<TokenType>(tokenTypes).Contains(topToken.TokenType);
    }

    public static bool PeekFor(this Queue<Token> tokenStream, TokenType tokenType)
        => tokenStream.Peek().TokenType == tokenType;

    public static bool PeekFor(this Queue<Token> tokenStream, TokenType[] tokenTypes)
        => new HashSet<TokenType>(tokenTypes).Contains(tokenStream.Peek().TokenType);
}