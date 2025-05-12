using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmNeg(AsmOperand Src) : AsmInstruction
{
    public override int OperandCount => 1;

    public override string EmitIrString() => $"Unary(Neg, {Src.EmitIrString()})";

    public override string EmitArmString() => $"        neg     {Src.EmitArmString()}, {Src.EmitArmString()}";

    public override AsmOperand? Operand(int n)
    => n switch
    {
        1 => Src,
        _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
    };

}