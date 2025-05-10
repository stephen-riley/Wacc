namespace Wacc.CodeGen.AbstractAsm;

public record AsmBitComplement(AsmOperandReg Dest, AsmOperandReg Src) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"mov {Dest.EmitString()}, {Src.EmitString()}";
}