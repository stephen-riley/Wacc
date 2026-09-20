using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record DoLoop(AstNode BodyBlock, AstNode CondExpr, string? Label = null, bool HasBreak = false, bool HasContinue = false) : AstNode
{
    public const string DefaultLabel = "$__TODO_DO_LABEL__";

    public VarMap? VariableMap;

    public override bool IsBlockItem() => true;

    public new static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(DoKw);

    public new static DoLoop Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(DoKw);

        var bodyNode = Block.Parse(tokenStream);
        var bodyBlock = bodyNode is Block || bodyNode.IsBlockItem() ? bodyNode : throw new ParseError($"{bodyNode} is not a Block or BlockItem");

        tokenStream.Expect(WhileKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        return new DoLoop(bodyBlock, condExpr);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"DoLoop({Label ?? DefaultLabel}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"body={BodyBlock.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"cond={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [BodyBlock, CondExpr];
}