using System.Text;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class BinaryOp(string op, IAstNode lExpr, IAstNode rExpr) : IAstNode
{
    public string Op => op;
    public IAstNode LExpr => lExpr;
    public IAstNode RExpr => rExpr;

    public static readonly HashSet<string> ShortCircuitOps = ["&&", "||"];

    public static readonly HashSet<string> RelationalOps = ["==", "!=", ">", ">=", "<", "<="];

    // Reference: https://en.cppreference.com/w/c/language/operator_precedence
    public static readonly Dictionary<TokenType, int> Precedence = new() {
        { MulSign, 50 },
        { DivSign, 50 },
        { ModSign, 50 },
        { PlusSign, 45 },
        { MinusSign, 45 },
        { BitwiseLeft, 40 },
        { BitwiseRight, 40 },
        { BitwiseAnd, 35 },
        { LessThan,35 },
        { LessOrEqual, 35 },
        { GreaterThan, 35 },
        { GreaterOrEqual, 35 },
        { BitwiseXor, 34 },
        { BitwiseOr, 33} ,
        { EqualTo, 30 },
        { NotEqualTo, 30 },
        { LogicalAnd, 10 },
        { LogicalOr, 5 },
    };

    public static readonly HashSet<TokenType> Operators = [.. Precedence.Keys];

    public static bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"Binary('{Op}'\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(lExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(rExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}