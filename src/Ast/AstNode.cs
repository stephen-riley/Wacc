using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Ast;

public abstract record AstNode
{
    public static AstNode Parse(Queue<Token> tokenStream) => throw new ParseException();

    public static Token Expect(TokenType[] tokenTypes, Queue<Token> tokenStream)
        => TryExpect(tokenTypes, tokenStream, out var topToken)
            ? topToken
            : throw new ParseException($"Expected {string.Join('/', tokenTypes)}, saw {topToken?.TokenType}");

    public static Token Expect(TokenType tokenType, Queue<Token> tokenStream)
        => TryExpect([tokenType], tokenStream, out var topToken)
            ? topToken
            : throw new ParseException($"Expected {tokenType}, saw {topToken?.TokenType}");

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

        topToken = null;
        return false;
    }
}