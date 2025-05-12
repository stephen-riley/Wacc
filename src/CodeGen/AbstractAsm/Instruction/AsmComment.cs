using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmComment(string Comment) : AsmInstruction
{
    public override int OperandCount => 0;

    public override string EmitArmString() => $"        ; {Comment}";

    // public override string EmitIrString() => $"Comment(\"{Comment}\")";
    public override string EmitIrString() => $"\n; {Comment}\n";

    public override AsmOperand? Operand(int n) => throw new CodeGenError("comments have no operands");
}