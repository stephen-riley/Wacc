namespace Wacc.Tacky.Instruction;

public abstract record TacVal() : ITackyInstr
{
    public TacVar? Dst() => null;
}