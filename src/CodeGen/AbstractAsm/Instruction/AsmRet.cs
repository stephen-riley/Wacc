using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmRet(string FuncName) : AsmInstruction
{
    public override int OperandCount => 0;

    public override string EmitIrString() => "Ret";

    public override string EmitArmString() => $"        b       _X{FuncName}";

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError("ret has no operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}