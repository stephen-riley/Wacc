namespace Wacc.Tacky.Instruction;

public record TacFunctionCall(string Name) : ITackyInstr
{
    public List<TacVar> Locals { get; } = [];

    public override string ToString() => $"; jsr {Name}";
}