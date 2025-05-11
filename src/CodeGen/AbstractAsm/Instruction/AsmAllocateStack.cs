namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmAllocateStack(int Size) : AsmInstruction
{
    public override string EmitString() => $"        AllocateStack({Size})";
}