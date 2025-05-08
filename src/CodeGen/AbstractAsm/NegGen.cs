namespace Wacc.CodeGen.AbstractAsm;

public record NegGen(OperandRegGen Dest, OperandRegGen Src) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"mov {Dest.EmitString()}, {Src.EmitString()}";
}