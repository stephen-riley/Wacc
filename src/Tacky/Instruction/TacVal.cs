namespace Wacc.Tacky.Instruction;

public closed record TacVal() : ITackyInstr
{
    public TacVar? Dst() => null;
}