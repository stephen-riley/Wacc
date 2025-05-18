using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmNewline() : AsmInstruction
{
    public override int OperandCount => 0;

    public override string EmitArmString() => "";

    public override string EmitIrString() => "";

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError("newlines have no operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}