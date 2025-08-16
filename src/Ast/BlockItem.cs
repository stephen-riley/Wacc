using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Ast;

public record BlockItem : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => Declaration.CanParse(tokenStream)
        || Statement.CanParse(tokenStream);

    // `isDependent` flags whether we're parsing a single statement inside an `if` or `while`
    public static IAstNode Parse(Queue<Token> tokenStream, bool isDependent = false)
    {
        if (!isDependent && Declaration.CanParse(tokenStream))
        {
            return Declaration.Parse(tokenStream);
        }
        else
        {
            return Statement.Parse(tokenStream);
        }
    }

    public IEnumerable<IAstNode> Children()
    {
        throw new NotImplementedException();
    }

    public virtual string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");
}