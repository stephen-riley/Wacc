namespace Wacc.Tacky.Instruction;

public record TacReturn(TacVal Val) : ITackyInstr
{
    public TacVar? Dst() => null;

    public override string ToString() => $"Return({Val})";
}