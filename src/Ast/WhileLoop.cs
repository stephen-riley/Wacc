using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using Wacc.Validation;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record WhileLoop(IAstNode CondExpr, IAstNode BodyBlock, string? Label = null) : IAstNode
{
    public const string DefaultLabel = "$__TODO_WHILE_LABEL__";

    public VarMap? VariableMap;

    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(WhileKw);

    public static WhileLoop Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(WhileKw);
        tokenStream.Expect(OpenParen);
        var condExpr = Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var bodyNode = Block.Parse(tokenStream, isDependent: true);
        var bodyBlock = bodyNode is Block || bodyNode.IsBlockItem() ? bodyNode : throw new ParseError($"{bodyNode} is not a Block or BlockItem");

        return new WhileLoop(condExpr, bodyBlock);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"WhileLoop({Label ?? DefaultLabel}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"cond={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"body={BodyBlock.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [CondExpr, BodyBlock];
}