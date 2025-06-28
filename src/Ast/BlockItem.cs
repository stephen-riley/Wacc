using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Ast;

public class BlockItem : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => Statement.CanParse(tokenStream) || Declaration.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        if (Statement.CanParse(tokenStream))
        {
            return Statement.Parse(tokenStream);
        }
        else if (Declaration.CanParse(tokenStream))
        {
            return Declaration.Parse(tokenStream);
        }
        else
        {
            throw new ParseError($"cannot parse BlockItem from {tokenStream.NextFewTokens()}");
        }
    }

    public string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");
}