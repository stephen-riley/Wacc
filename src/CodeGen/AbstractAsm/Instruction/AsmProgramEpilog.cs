namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmProgramEpilog() : AsmInstruction
{
    public override string EmitIrString() => "ProgramEpilog";

    public override string EmitArmString() => "        ; program epilog here";
}