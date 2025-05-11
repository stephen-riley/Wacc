using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmLoadStack(AsmStackOperand Src, AsmDestOperand Dst) : AsmInstruction
{
    public override string EmitIrString() => $"LoadStack({Src.EmitIrString()}, {Dst.EmitIrString()})";

    public override string EmitArmString() => $"        ldr     {Dst.EmitArmString()}, {Src.EmitArmString()}";
}