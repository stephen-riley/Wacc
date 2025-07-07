using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Ast;

public record BlockItem : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => Statement.CanParse(tokenStream) || Declaration.CanParse(tokenStream);

    // `isDependent` flags whether we're parsing a single statement inside an `if` or `while`
    public static IAstNode Parse(Queue<Token> tokenStream, bool isDependent = false)
    {
        if (Statement.CanParse(tokenStream))
        {
            return Statement.Parse(tokenStream);
        }
        else if (!isDependent && Declaration.CanParse(tokenStream))
        {
            return Declaration.Parse(tokenStream);
        }
        else
        {
            throw new ParseError($"cannot parse BlockItem from {tokenStream.NextFewTokens()}");
        }
    }

    public virtual string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");
}