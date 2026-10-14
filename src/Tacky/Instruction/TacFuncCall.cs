namespace Wacc.Tacky.Instruction;

public record TacFuncCall(string Name, TacVar Dst) : ITackyInstr
{
    public List<TacVar> Locals { get; } = [];

    public override string ToString() => $"FuncCall({Name}, {Dst})";
}