namespace Wacc.CodeGen.AbstractAsm;

public record MovGen(IAbstractAsm Exp, OperandRegGen Reg) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"movl {Exp.EmitString()} {Reg.EmitString()}";
}