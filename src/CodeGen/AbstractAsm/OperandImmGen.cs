namespace Wacc.CodeGen.AbstractAsm;

public record OperandImmGen(int Imm) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => Imm.ToString();
}