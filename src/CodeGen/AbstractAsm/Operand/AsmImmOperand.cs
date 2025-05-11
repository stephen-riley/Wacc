namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmImmOperand(int Imm) : AsmOperand
{
    public override string EmitString() => Imm.ToString();
}