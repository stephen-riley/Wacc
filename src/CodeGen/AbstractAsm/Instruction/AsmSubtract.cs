using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmSubtract(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) : AsmInstruction
{
    public override int OperandCount => 3;

    public override string EmitArmString() => $"        sub     {Dst.EmitArmString()}, {Src1.EmitArmString()}, {Src2.EmitArmString()}";

    public override string EmitIrString() => $"Sub({Src1.EmitIrString()}, {Src2.EmitIrString()}, {Dst.EmitIrString()})";

    public override AsmOperand? GetOperand(int n)
        => n switch
        {
            1 => Src1,
            2 => Src2,
            3 => Dst,
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => n switch
        {
            1 => this with { Src1 = o },
            2 => this with { Src2 = o },
            3 => this with { Dst = (AsmDestOperand)o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}