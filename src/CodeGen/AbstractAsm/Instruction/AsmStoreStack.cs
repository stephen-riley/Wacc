using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmStoreStack(AsmOperand Src, AsmStackOperand Dst) : AsmInstruction
{
    public override string EmitIrString() => $"StoreStack({Src.EmitIrString()}, {Dst.EmitIrString()})";

    public override string EmitArmString() => $"        str     {Src.EmitArmString()}, {Dst.EmitArmString()}";
}