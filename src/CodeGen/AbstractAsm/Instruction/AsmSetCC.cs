using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmSetCC(AsmCmp.CondCode CondCode, AsmOperand Src) : AsmInstruction
{
    public override int OperandCount => 0;

    // This is a no-op for Arm64 assembly, but we still have it because the book does.
    public override string EmitArmString() => $"";

    public override string EmitIrString() => $"SetCC({CondCode}, {Src.EmitIrString()})";

    public override AsmOperand? GetOperand(int n)
        => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}