namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmRegOperand(Register Reg) : AsmDestOperand
{
    public override string EmitArmString() => Reg.ToString().ToLower();

    public override string EmitIrString() => $"Reg({Reg.ToString().ToLower()})";
}