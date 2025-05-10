namespace Wacc.CodeGen.AbstractAsm;

public record AsmBitNot(AsmPseudoOperand Src) : IAbstractAsm
{
    public string EmitString() => $"    Unary(Not, {Src.EmitString()})";
}