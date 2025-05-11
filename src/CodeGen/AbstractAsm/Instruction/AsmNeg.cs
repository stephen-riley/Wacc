using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmNeg(AsmOperand Src) : AsmInstruction
{
    public override string EmitIrString() => $"        Unary(Neg, {Src.EmitIrString()})";
}