namespace Wacc.Tacky.Instruction;

public record TacProgram(IEnumerable<TacFunction> Functions) : ITackyInstr
{
    public TacVar? Dst() => null;

    public override string ToString() => $"; Program()";
}