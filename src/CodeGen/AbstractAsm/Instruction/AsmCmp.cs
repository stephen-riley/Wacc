using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmCmp(AsmOperand Src1, AsmOperand Src2) : AsmInstruction
{
    public enum CondCode
    {
        EQ,
        NE,
        GE,
        GT,
        LE,
        LT
    }

    public override int OperandCount => 2;

    public override string EmitArmString() => $"        cmp     {Src1.EmitArmString()}, {Src2.EmitArmString()}";

    public override string EmitIrString() => $"Cmp({Src1.EmitIrString()}, {Src2.EmitIrString()})";

    public override AsmOperand? GetOperand(int n)
        => n switch
        {
            1 => Src1,
            2 => Src2,
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => n switch
        {
            1 => this with { Src1 = o },
            2 => this with { Src2 = o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}