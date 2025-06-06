namespace Wacc.Tacky.Instruction;

public record TacJumpIfNotZero(TacVal Src, string Identifier) : ITackyInstr
{
    public override string ToString() => $"JumpIfNotZero({Src}, \"{Identifier}\")";
}