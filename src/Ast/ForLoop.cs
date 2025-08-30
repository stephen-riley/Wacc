using System.Text;
using Wacc.Exceptions;
using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record ForLoop(IAstNode InitStat, IAstNode CondExpr, IAstNode UpdateStat, IAstNode BodyBlock) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => tokenStream.PeekFor(ForKw);

    public static ForLoop Parse(Queue<Token> tokenStream)
    {
        tokenStream.Expect(ForKw);
        tokenStream.Expect(OpenParen);

        var initNode = ForInit.Parse(tokenStream);
        // Semicolon is handled by `Forinit`. See `ForInit.Parse` for details.

        var condNode = tokenStream.PeekFor(Semicolon) ? new Block([]) : Expression.Parse(tokenStream);
        tokenStream.Expect(Semicolon);

        var updateNode = tokenStream.PeekFor(CloseParen) ? new Block([]) : Expression.Parse(tokenStream);
        tokenStream.Expect(CloseParen);

        var bodyNode = Block.Parse(tokenStream, isDependent: true);
        var bodyBlock = bodyNode is Block || bodyNode.IsBlockItem() ? bodyNode : throw new ParseError($"{bodyNode} is not a Block or BlockItem");

        return new ForLoop(initNode, condNode, updateNode, bodyBlock);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ForLoop(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"init={InitStat.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"init={CondExpr.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"init={UpdateStat.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"body={BodyBlock.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [InitStat, CondExpr, UpdateStat, BodyBlock];
}