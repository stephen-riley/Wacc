namespace Wacc.CodeGen.AbstractAsm;

public record AsmRegOperand(Register Reg) : AsmOperand
{
    public override string EmitString() => Reg.ToString().ToLower();
}