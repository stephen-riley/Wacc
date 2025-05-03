namespace Wacc.CodeGen.AbstractAsm;

public record MovGen(OperandRegGen Reg, IAbstractAsm Exp) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"mov {Reg.EmitString()}, {Exp.EmitString()}";
}