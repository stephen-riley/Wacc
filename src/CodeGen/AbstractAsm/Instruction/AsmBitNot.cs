using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmBitNot(AsmOperand Src) : AsmInstruction
{
    public override string EmitString() => $"        Unary(Not, {Src.EmitString()})";
}