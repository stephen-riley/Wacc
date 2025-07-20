using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Ast;

public record BlockItem : IAstNode
{
    public static bool CanParse(Queue<Token> tokenStream)
        => LabeledStatement.CanParse(tokenStream)
        || Statement.CanParse(tokenStream)
        || Declaration.CanParse(tokenStream);

    // `isDependent` flags whether we're parsing a single statement inside an `if` or `while`
    public static IAstNode Parse(Queue<Token> tokenStream, bool isDependent = false)
    {
        Label? label = Label.CanParse(tokenStream) ? (Label)Label.Parse(tokenStream) : null;

        IAstNode stat = null!;

        if (Statement.CanParse(tokenStream))
        {
            stat = Statement.Parse(tokenStream);
        }
        else if (!isDependent && Declaration.CanParse(tokenStream))
        {
            stat = Declaration.Parse(tokenStream);
        }
        else
        {
            throw new ParseError($"cannot parse BlockItem from {tokenStream.NextFewTokens()}");
        }

        return label is null ? stat : new LabeledStatement(label, stat);
    }

    public IEnumerable<IAstNode> Children()
    {
        throw new NotImplementedException();
    }

    public virtual string ToPrettyString(int indent = 0) => throw new ParseError($"{GetType().Name}.{nameof(ToPrettyString)} should not be called");
}