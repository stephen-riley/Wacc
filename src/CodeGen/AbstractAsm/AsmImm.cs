namespace Wacc.CodeGen.AbstractAsm;

public record AsmImm(int Int) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"#{Int}";
}