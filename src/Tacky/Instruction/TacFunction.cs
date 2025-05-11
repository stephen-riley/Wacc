namespace Wacc.Tacky.Instruction;

public record TacFunction(string Name, IEnumerable<ITackyInstr> Instructions) : ITackyInstr
{
    public List<TacVar> Locals { get; } = [];

    public override string ToString() => $"; function {Name}";
}