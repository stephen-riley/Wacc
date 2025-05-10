namespace Wacc.CodeGen.AbstractAsm;

public record AsmOperandImm(int Imm) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => Imm.ToString();
}