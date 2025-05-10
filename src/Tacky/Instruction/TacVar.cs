namespace Wacc.Tacky.Instruction;

public record TacVar(string Name) : TacVal
{
    public override string ToString() => $"Var(\"{Name}\")";
}