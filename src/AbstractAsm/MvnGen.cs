namespace Wacc.AbstractAsm;

public record MvnGen(OperandRegGen Dest, OperandRegGen Src) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"mov {Dest.EmitString()}, {Src.EmitString()}";
}