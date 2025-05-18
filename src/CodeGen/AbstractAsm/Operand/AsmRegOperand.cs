namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmRegOperand(ArmReg Reg) : AsmDestOperand
{
    public override string EmitArmString() => Reg.ToString().ToLower();

    public override string EmitIrString() => $"Reg({Reg.ToString().ToLower()})";
}