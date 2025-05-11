namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmImmOperand(int Imm) : AsmOperand
{
    public override string EmitArmString() => $"#{Imm}";

    public override string EmitIrString() => Imm.ToString();
}