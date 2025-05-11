namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmStackOperand(int Offset) : AsmDestOperand
{
    public override string EmitArmString() => $"[fp, #{Offset}]";

    public override string EmitIrString() => $"Stack({Offset})";
}