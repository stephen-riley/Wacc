namespace Wacc.Tacky.Instruction;

public record TacUnary(string Op, TacVal Src, TacVar Dst) : ITackyInstr
{
    public string OpName => Op switch
    {
        "-" => "Negate",
        "~" => "Complement",
        "!" => "Not",
        _ => throw new InvalidOperationException($"no unary operator '{Op}'")
    };

    public TacVar GetDst() => Dst;

    public override string ToString() => $"Unary({OpName}, {Src}, {Dst})";
}