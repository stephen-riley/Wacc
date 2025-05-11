namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmRet() : AsmInstruction
{
    public override string EmitIrString() => "Ret";

    public override string EmitArmString() => $"        ret";
}