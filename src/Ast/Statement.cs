using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Statement() : IAstNode
{
    // This will list the statement types that may not have semicolons after them.
    public static readonly HashSet<TokenType> BlockStatements = [DefaultKw, CaseKw, DoKw, ForKw, IfKw, SwitchKw, WhileKw];

    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream)
        => Break.CanParse(tokenStream)
            || Case.CanParse(tokenStream)
            || Continue.CanParse(tokenStream)
            || Default.CanParse(tokenStream)
            || DoLoop.CanParse(tokenStream)
            || Expression.CanParse(tokenStream)
            || ForLoop.CanParse(tokenStream)
            || Goto.CanParse(tokenStream)
            || IfElse.CanParse(tokenStream)
            || Label.CanParse(tokenStream)
            || NullStatement.CanParse(tokenStream)
            || Return.CanParse(tokenStream)
            || Switch.CanParse(tokenStream)
            || WhileLoop.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream, bool nested = false)
    {
        if (Label.CanParse(tokenStream))
        {
            return LabeledBlock.Parse(tokenStream);
        }

        var nextTokenType = tokenStream.Peek().TokenType;

        var stat = nextTokenType switch
        {
            BreakKw => Break.Parse(tokenStream),
            CaseKw => Case.Parse(tokenStream),
            ContinueKw => Continue.Parse(tokenStream),
            DefaultKw => Default.Parse(tokenStream),
            DoKw => DoLoop.Parse(tokenStream),
            ForKw => ForLoop.Parse(tokenStream),
            GotoKw => Goto.Parse(tokenStream),
            IfKw => IfElse.Parse(tokenStream),
            ReturnKw => Return.Parse(tokenStream),
            Semicolon => NullStatement.Parse(tokenStream),
            SwitchKw => Switch.Parse(tokenStream),
            WhileKw => WhileLoop.Parse(tokenStream),
            _ => Expression.Parse(tokenStream)
        };

        if (!nested && !BlockStatements.Contains(nextTokenType))
        {
            tokenStream.Expect(Semicolon);
        }

        return stat;
    }

    public string ToPrettyString(int indent = 0)
        => throw new NotImplementedException($"{GetType().Name}.{nameof(ToPrettyString)}");

    public IEnumerable<IAstNode> Children() => [];
}