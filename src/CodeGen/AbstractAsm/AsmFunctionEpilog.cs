namespace Wacc.CodeGen.AbstractAsm;

public record AsmFunctionEpilog(string Name) : IAbstractAsm
{
    public string EmitString() =>
        "\n" +
        "    .cfi_endproc\n" +
        $"    ; end function {Name}";
}