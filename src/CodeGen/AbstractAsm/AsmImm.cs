namespace Wacc.CodeGen.AbstractAsm;

public record AsmImm(int Int) : IAbstractAsm
{
    public string EmitString() => $"#{Int}";
}