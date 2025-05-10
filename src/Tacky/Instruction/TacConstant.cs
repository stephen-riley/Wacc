namespace Wacc.Tacky.Instruction;

public record TacConstant(int Value) : TacVal
{
    public override string ToString() => $"Constant({Value})";
}