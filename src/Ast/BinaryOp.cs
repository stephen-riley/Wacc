using System.Text;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Ast;

public record BinaryOp(TokenType Op, AstNode LExpr, AstNode RExpr) : AstNode
{
    // public TokenType Op => op;
    // public IAstNode LExpr => lExpr;
    // public IAstNode RExpr => rExpr;

    public static readonly HashSet<TokenType> ShortCircuitOps = [LogicalAnd, LogicalOr];

    public static readonly HashSet<TokenType> RelationalOps = [EqualTo, NotEqualTo, GreaterThan, GreaterOrEqual, LessThan, LessOrEqual];

    public static readonly HashSet<TokenType> RightAssociativeOps = [
        Assign, CompoundPlus, CompoundMinus, CompoundMul, CompoundDiv, CompoundMod,
        CompoundBitwiseAnd, CompoundBitwiseOr, CompoundBitwiseXor, CompoundBitwiseLeft, CompoundBitwiseRight,
        Question, Colon,
        // Increment and Decrement are not here because they are handled in `UnaryOp.Parse()`.
    ];

    // Precedence climbing table
    //  levels and values from https://en.cppreference.com/w/c/language/operator_precedence
    public static readonly Dictionary<TokenType, int> Precedence = new() {
        // level 3
        { Asterisk, 50 },
        { Div, 50 },
        { Mod, 50 },

        // level 4
        { Plus, 45 },
        { Minus, 45 },

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

        // level 13
        { Question, 3 },

        // level 14
        { Assign, 1 },
        { CompoundPlus, 1 },
        { CompoundMinus, 1 },
        { CompoundMul, 1 },
        { CompoundDiv, 1 },
        { CompoundMod, 1 },
        { CompoundBitwiseAnd, 1 },
        { CompoundBitwiseOr, 1 },
        { CompoundBitwiseXor, 1 },
        { CompoundBitwiseLeft, 1 },
        { CompoundBitwiseRight, 1 },
    };

    public static readonly HashSet<TokenType> Operators = [.. Precedence.Keys];

    public new bool CanParse(Queue<Token> tokenStream) => throw new InvalidOperationException("should not be called");

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.Append($"Binary('{Op.Description()}'\n");
        sb.Append(AstNode.IndentStr(indent + 1)).Append(LExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(AstNode.IndentStr(indent + 1)).Append(RExpr.ToPrettyString(indent + 1)).Append('\n');
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [];
}