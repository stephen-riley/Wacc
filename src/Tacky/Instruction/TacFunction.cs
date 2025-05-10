namespace Wacc.Tacky.Instruction;

public record TacFunction(string Name, IEnumerable<ITackyInstr> Instructions) : ITackyInstr
{
    public TacVar? Dst() => null;

    public override string ToString() => $"; function {Name}";
}