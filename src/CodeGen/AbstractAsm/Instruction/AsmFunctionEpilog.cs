namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmFunctionEpilog(string Name) : AsmInstruction
{
    public override string EmitString() =>
        "\n" +
        "        .cfi_endproc\n" +
        $"        ; end function _{Name}";
}