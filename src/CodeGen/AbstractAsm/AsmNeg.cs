namespace Wacc.CodeGen.AbstractAsm;

public record AsmNeg(AsmPseudoOperand Src) : IAbstractAsm
{
    public string EmitString() => $"    Unary(Neg, {Src.EmitString()})";
}