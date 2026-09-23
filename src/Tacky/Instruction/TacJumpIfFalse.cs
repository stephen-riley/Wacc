namespace Wacc.Tacky.Instruction;

public record TacJumpIfFalse(TacVal Src, string Identifier) : ITackyInstr
{
    public override string ToString() => $"JumpIfFalse({Src}, \"{Identifier}\")";
}