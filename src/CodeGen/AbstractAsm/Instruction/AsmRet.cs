using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmRet() : AsmInstruction
{
    public override int OperandCount => 0;

    public override string EmitIrString() => "Ret";

    public override string EmitArmString() => $"        ret";

    public override AsmOperand? Operand(int n) => throw new CodeGenError("ret has no operands");
}