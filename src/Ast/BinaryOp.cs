using System.Text;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public class BinaryOp(TokenType op, IAstNode lExpr, IAstNode rExpr) : IAstNode
{
    public TokenType Op => op;
    public IAstNode LExpr => lExpr;
    public IAstNode RExpr => rExpr;

    // TODO: change these string literals to TokenTypes
    public static readonly HashSet<TokenType> ShortCircuitOps = [LogicalAnd, LogicalOr];

    public static readonly HashSet<TokenType> RelationalOps = [EqualTo, NotEqualTo, GreaterThan, GreaterOrEqual, LessThan, LessOrEqual];

    public static readonly HashSet<TokenType> RightAssociativeOps = [Assign, PlusAssign, MinusAssign, MulAssign, DivAssign, ModAssign];

    // Precedence climbing table
    //  levels and values from https://en.cppreference.com/w/c/language/operator_precedence
    public static readonly Dictionary<TokenType, int> Precedence = new() {
        // level 3
        { MulSign, 50 },
        { DivSign, 50 },
        { ModSign, 50 },

        // level 4
        { PlusSign, 45 },
        { MinusSign, 45 },

        // level 5
        { BitwiseLeft, 40 },
        { BitwiseRight, 40 },

        // level 6
        { LessThan,35 },
        { LessOrEqual, 35 },
        { GreaterThan, 35 },
        { GreaterOrEqual, 35 },

        // level 7
        { EqualTo, 30 },
        { NotEqualTo, 30 },

        // level 8
        { BitwiseAnd, 25 },

        // level 9
        { BitwiseXor, 20 },

        // level 10
        { BitwiseOr, 15 },

        // level 11
        { LogicalAnd, 10 },

        // level 12
        { LogicalOr, 5 },

        // level 14
        { Assign, 1 },
        { PlusAssign, 1 },
        { MinusAssign, 1 },
        { MulAssign, 1 },
        { DivAssign, 1 },
        { ModAssign, 1 },
    };

    public static readonly HashSet<TokenType> Operators = [.. Precedence.Keys];

    public static bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"Binary('{Op.Description()}'\n");
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(lExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent + 1)).Append(rExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }
}