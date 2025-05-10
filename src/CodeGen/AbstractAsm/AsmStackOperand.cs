namespace Wacc.CodeGen.AbstractAsm;

public record AsmStackOperand(int Offset) : AsmOperand
{
    public override string EmitString() => $"Stack({Offset})";
}