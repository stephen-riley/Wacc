namespace Wacc.CodeGen.AbstractAsm;

public record AsmMov(AsmOperand Src, AsmPseudoOperand Dst) : IAbstractAsm
{
    public string EmitString() => $"    Mov({Src.EmitString()}, {Dst.EmitString()})";
}