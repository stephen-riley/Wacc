namespace Wacc.CodeGen.AbstractAsm;

public record AsmAllocateStack(int Size) : IAbstractAsm
{
    public string EmitString() => $"    AllocateStack({Size})";
}