namespace Wacc.Tacky.Instruction;

public record TacBinary(string Op, TacVal Src1, TacVal Src2, TacVar Dst) : ITackyInstr
{
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