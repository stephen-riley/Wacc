namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmFunctionEpilog(string Name) : AsmInstruction
{
    public override string EmitIrString() => $"FuncEpilog({Name})";

    public override string EmitArmString() =>
        "\n" +
        "        .cfi_endproc\n" +
        $"        ; end function _{Name}";
}