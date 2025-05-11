namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmRegOperand(Register Reg) : AsmDestOperand
{
    public override string EmitString() => $"Reg({Reg.ToString().ToLower()})";
}