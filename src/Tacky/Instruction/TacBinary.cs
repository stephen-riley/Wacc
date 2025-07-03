using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.Tokens;
using static Wacc.CodeGen.AbstractAsm.Instruction.AsmCmp.CondCode;
using static Wacc.Tokens.TokenType;

namespace Wacc.Tacky.Instruction;

public record TacBinary(TokenType Op, TacVal Src1, TacVal Src2, TacVar Dst) : ITackyInstr
{
    public static readonly Dictionary<TokenType, AsmCmp.CondCode> CondCode = new() {
        { EqualTo, EQ },
        { NotEqualTo, NE },
        { LessThan, LT },
        { LessOrEqual, LE },
        { GreaterThan, GT },
        { GreaterOrEqual, GE }
    };

    public string OpName => Op switch
    {
        PlusSign => "Add",
        MinusSign => "Subtract",
        MulSign => "Multiply",
        DivSign => "Divide",
        ModSign => "Modulus",
        BitwiseAnd => "BitwiseAnd",
        BitwiseOr => "BitwiseOr",
        BitwiseLeft => "BitwiseLeft",
        BitwiseRight => "BitwiseRight",
        BitwiseXor => "BitwiseXor",
        EqualTo => "Equal",
        NotEqualTo => "NotEqual",
        LessThan => "LessThan",
        LessOrEqual => "LessOrEqual",
        GreaterThan => "GreaterThan",
        GreaterOrEqual => "GreaterOrEqual",
        _ => throw new InvalidOperationException($"no binary operator '{Op}'")
    };

    public TacVar GetDst() => Dst;

    public override string ToString() => $"Binary({OpName}, {Src1}, {Src2}, {Dst})";
}