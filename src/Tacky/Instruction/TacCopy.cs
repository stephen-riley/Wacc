namespace Wacc.Tacky.Instruction;

public record TacCopy(TacVal Src, TacVar Dst) : ITackyInstr
{
    public TacVar GetDst() => Dst;

    public override string ToString() => $"Copy({Src}, {Dst})";
}