namespace Wacc.Tacky.Instruction;

public record TacJump(string Identifier) : ITackyInstr
{
    public override string ToString() => $"Jump(\"{Identifier}\")";
}