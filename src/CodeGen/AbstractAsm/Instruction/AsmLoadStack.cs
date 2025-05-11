using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmLoadStack(AsmStackOperand Src, AsmDestOperand Dst) : AsmInstruction
{
    public override string EmitString() => $"        LoadStack({Src.EmitString()}, {Dst.EmitString()})";
}