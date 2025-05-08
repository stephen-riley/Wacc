namespace Wacc.Tacky.Instruction

public record TacUnary(string Op, TacVal Src, TacVal Dest)
{
    public const string Complement = "~";
    public const string Negate = "-";
}