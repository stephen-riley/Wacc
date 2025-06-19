using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmSetCC(AsmCmp.CondCode CondCode, AsmDestOperand Dst) : AsmInstruction
{
    public override int OperandCount => 1;

    public override string EmitArmString() => $"        cset    {Dst.EmitArmString()}, {CondCode}";

    public override string EmitIrString() => $"SetCC({CondCode}, {Dst.EmitIrString()})";

    public override AsmOperand? GetOperand(int n)
        => n switch
        {
            1 => Dst,
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => n switch
        {
            1 => this with { Dst = (AsmDestOperand)o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}