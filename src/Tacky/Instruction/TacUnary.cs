namespace Wacc.Tacky.Instruction;

public record TacUnary(string Op, TacVal Src, TacVar Dest) : ITackyInstr
{
    public string OpName => Op switch
    {
        "-" => "Negate",
        "~" => "Complement",
        _ => throw new InvalidOperationException($"no unary operator '{Op}'")
    };

    public TacVar GetDst() => Dest;

    public override string ToString() => $"Unary({OpName}, {Src}, {Dest})";
}