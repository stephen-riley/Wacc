using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmDeallocateStack(int Size) : AsmInstruction
{
    public override string EmitIrString() => $"DeallocateStack({Size})";

    public override string EmitArmString() =>
$"""
        add     sp, sp, #{Size}             ; deallocate stack frame
""";

    public override int OperandCount => 0;

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}