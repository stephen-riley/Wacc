using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmJmpCC(AsmCmp.CondCode CondCode, string Label) : AsmInstruction
{
    public override int OperandCount => 0;

    public override string EmitArmString()
        => $"        b.{CondCode}    {Label}";


    public override string EmitIrString() => $"JmpCC({CondCode}, \"{Label}\")";

    public override AsmOperand? GetOperand(int n)
        => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}