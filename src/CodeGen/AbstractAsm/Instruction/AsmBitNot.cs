using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmBitNot(AsmOperand Src) : AsmInstruction
{
    public override string EmitIrString() => $"Unary(Not, {Src.EmitIrString()})";

    public override string EmitArmString() => $"        mvn     {Src.EmitArmString()}, {Src.EmitArmString()}";
}