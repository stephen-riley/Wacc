using System.Diagnostics.CodeAnalysis;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Parse;

public static class ParseExtentions
{
    public static bool PeekFor(this Queue<Token> tokenStream, TokenType tokenType, int depth = 1)
    {
        Token? token = depth == 1 ? tokenStream.Peek() : tokenStream.ElementAtOrDefault(depth - 1);
        return token is not null && token.TokenType == tokenType;
    }

    public static bool PeekFor(this Queue<Token> tokenStream, IEnumerable<TokenType> tokenTypes, int depth = 1)
    {
        Token? token = depth == 1 ? tokenStream.Peek() : tokenStream.ElementAtOrDefault(depth - 1);
        return token is not null && new HashSet<TokenType>(tokenTypes).Contains(token.TokenType);
    }

    public static bool TryExpect(this Queue<Token> tokenStream, IEnumerable<TokenType> tokenTypes, [NotNullWhen(true)] out Token topToken)
    {
        topToken = tokenStream.Dequeue();
        return new HashSet<TokenType>(tokenTypes).Contains(topToken.TokenType);
    }

    public static Token Expect(this Queue<Token> tokenStream, IEnumerable<TokenType> tokenTypes)
        => tokenStream.TryExpect(tokenTypes, out var topToken)
            ? topToken
            : throw new ParseError($"Expected {string.Join('/', tokenTypes)}, saw {topToken?.TokenType}«{topToken?.Str}»");

    public static Token Expect(this Queue<Token> tokenStream, TokenType tokenType)
        => tokenStream.TryExpect([tokenType], out var topToken)
            ? topToken
            : throw new ParseError($"Expected {tokenType}, saw {topToken?.TokenType}«{topToken?.Str}»");

    public static bool Is(this Token tok, TokenType tokenType) => tok.Is([tokenType]);

    public static bool Is(this Token tok, HashSet<TokenType> tokenTypes) => tokenTypes.Contains(tok.TokenType);
}