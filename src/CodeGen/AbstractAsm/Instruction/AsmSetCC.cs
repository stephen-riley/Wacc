using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmSetCC(AsmCmp.CondCode CondCode, AsmOperand Src) : AsmInstruction
{
    public override int OperandCount => 1;

    public override string EmitArmString() => $"        cset    {Src.EmitArmString()}, {CondCode}";

    public override string EmitIrString() => $"SetCC({CondCode}, {Src.EmitIrString()})";

    public override AsmOperand? GetOperand(int n)
        => n switch
        {
            1 => Src,
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => n switch
        {
            1 => this with { Src = o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}