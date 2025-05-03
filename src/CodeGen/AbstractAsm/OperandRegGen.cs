namespace Wacc.CodeGen.AbstractAsm;

public record OperandRegGen(Register Reg) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => "%" + Reg.ToString().ToLower();
}