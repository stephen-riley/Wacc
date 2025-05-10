namespace Wacc.CodeGen.AbstractAsm;

public record AsmImmOperand(int Imm) : AsmOperand
{
    public override string EmitString() => Imm.ToString();
}