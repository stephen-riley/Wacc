namespace Wacc.Tacky.Instruction;

public record TacJumpIfTrue(TacVal Src, string Identifier) : ITackyInstr
{
    public override string ToString() => $"JumpIfTrue({Src}, \"{Identifier}\")";
}