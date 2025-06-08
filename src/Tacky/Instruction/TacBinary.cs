using Wacc.CodeGen.AbstractAsm.Instruction;
using static Wacc.CodeGen.AbstractAsm.Instruction.AsmCmp.CondCode;

namespace Wacc.Tacky.Instruction;

public record TacBinary(string Op, TacVal Src1, TacVal Src2, TacVar Dst) : ITackyInstr
{
    public static readonly Dictionary<string, AsmCmp.CondCode> CondCode = new() {
        { "==", EQ },
        { "!=", NE },
        { "<", LT },
        { "<=", LE },
        { ">", GT },
        { ">=", GE }
    };

    public string OpName => Op switch
    {
        "+" => "Add",
        "-" => "Subtract",
        "*" => "Multiply",
        "/" => "Divide",
        "%" => "Modulus",
        "&" => "BitwiseAnd",
        "|" => "BitwiseOr",
        "<<" => "BitwiseLeft",
        ">>" => "BitwiseRight",
        "^" => "BitwiseXor",
        "==" => "Equal",
        "!=" => "NotEqual",
        "<" => "LessThan",
        "<=" => "LessOrEqual",
        ">" => "GreaterThan",
        ">=" => "GreaterOrEqual",
        _ => throw new InvalidOperationException($"no binary operator '{Op}'")
    };

    public TacVar GetDst() => Dst;

    public override string ToString() => $"Binary({OpName}, {Src1}, {Src2}, {Dst})";
}