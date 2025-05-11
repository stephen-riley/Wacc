namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmProgramEpilog() : AsmInstruction
{
    // TODO: might need this in the future
    public override string EmitString() => "";
}