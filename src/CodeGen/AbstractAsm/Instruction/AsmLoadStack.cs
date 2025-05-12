using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmLoadStack(AsmStackOperand Src, AsmDestOperand Dst) : AsmInstruction
{
    public override int OperandCount => 2;

    public override string EmitIrString() => $"LoadStack({Src.EmitIrString()}, {Dst.EmitIrString()})";

    public override string EmitArmString() => $"        ldr     {Dst.EmitArmString()}, {Src.EmitArmString()}";

    public override AsmOperand? Operand(int n)
    => n switch
    {
        1 => Src,
        2 => Dst,
        _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
    };

}