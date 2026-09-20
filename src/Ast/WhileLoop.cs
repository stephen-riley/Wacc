using System.Text;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public partial record WhileLoop(AstNode CondExpr, AstNode BodyBlock, string? Label = null) : AstNode
{
    public const string DefaultLabel = "$__TODO_WHILE_LABEL__";

    public VarMap? VariableMap;

    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(WhileKw);

    public new static WhileLoop Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(WhileKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);
        var bodyNode = Block.Parse(tokenStream);

        return new WhileLoop(condExpr, bodyNode);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"WhileLoop({Label ?? DefaultLabel}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"cond={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"body={BodyBlock.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [CondExpr, BodyBlock];
}