namespace Wacc.CodeGen.AbstractAsm.Operand;

public record AsmStackOperand(int Offset) : AsmDestOperand
{
    public override string EmitString() => $"Stack({Offset})";
}