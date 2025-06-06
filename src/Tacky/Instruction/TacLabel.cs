namespace Wacc.Tacky.Instruction;

public record TacLabel(string Identifier) : ITackyInstr
{
    public override string ToString() => $"Label(\"{Identifier}\")";
}