namespace Wacc.Tacky.Instruction;

public record TacReturn(TacVal Val) : ITackyInstr
{
    public override string ToString() => $"Return({Val})";
}