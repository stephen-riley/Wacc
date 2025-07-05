using System.Text;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record Assignment(IAstNode LExpr, IAstNode RExpr) : BlockItem
{
    public static readonly Dictionary<TokenType, TokenType> AssignOpMap = new()
    {
        [CompoundPlus] = PlusSign,
        [CompoundMinus] = MinusSign,
        [CompoundMul] = MulSign,
        [CompoundDiv] = DivSign,
        [CompoundMod] = ModSign,
        [CompoundBitwiseAnd] = BitwiseAnd,
        [CompoundBitwiseOr] = BitwiseOr,
        [CompoundBitwiseXor] = BitwiseXor,
        [CompoundBitwiseLeft] = BitwiseLeft,
        [CompoundBitwiseRight] = BitwiseRight,
    };

    public new static bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Assignment(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(LExpr.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine(RExpr.ToPrettyString(indent + 1));
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}