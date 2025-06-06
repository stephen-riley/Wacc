namespace Wacc.Tacky.Instruction;

public record TacJumpIfZero(TacVal Src, string Identifier) : ITackyInstr
{
    public override string ToString() => $"JumpIfZero({Src}, \"{Identifier}\")";
}