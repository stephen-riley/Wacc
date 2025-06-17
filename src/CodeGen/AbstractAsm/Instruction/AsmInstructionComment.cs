using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmInstructionComment(string? Comment = "") : AsmInstruction
{
    public override int OperandCount => 0;

    public override string EmitArmString() => $"        ;; {Comment}";

    public override string EmitIrString() => $"; {Comment}";

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError("comments have no operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}