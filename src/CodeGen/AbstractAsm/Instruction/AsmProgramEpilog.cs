using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmProgramEpilogue() : AsmInstruction
{
    public override string EmitIrString() => "ProgramEpilog";

    public override string EmitArmString() => "        ; program epilog here";

    public override int OperandCount => 0;

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}