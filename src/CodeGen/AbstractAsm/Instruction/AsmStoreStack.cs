using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmStoreStack(AsmOperand Src, AsmStackOperand Dst) : AsmInstruction
{
    public override string EmitString() => $"        StoreStack({Src.EmitString()}, {Dst.EmitString()})";
}