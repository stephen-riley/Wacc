using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmMov(AsmOperand Src, AsmDestOperand Dst) : AsmInstruction
{
    public override string EmitIrString() => $"        Mov({Src.EmitIrString()}, {Dst.EmitIrString()})";
}