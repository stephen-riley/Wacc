namespace Wacc.AbstractAsm;

public record ImmGen(int Int) : IAbstractAsm
{
    public void Emit(TextWriter stream) => Console.Write(EmitString());

    public string EmitString() => $"#{Int}";
}