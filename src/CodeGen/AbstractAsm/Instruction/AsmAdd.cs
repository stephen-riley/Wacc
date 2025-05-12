using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmAdd(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) : AsmInstruction
{
    public override int OperandCount => 3;

    public override string EmitArmString() => $"        add     {Dst.EmitArmString()}, {Src1.EmitArmString()}, {Src2.EmitArmString()}";

    public override string EmitIrString() => $"Add({Src1.EmitIrString()}, {Src2.EmitIrString()}, {Dst.EmitIrString()})";

    public override AsmOperand? Operand(int n)
        => n switch
        {
            1 => Src1,
            2 => Src2,
            3 => Dst,
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}