namespace Wacc.CodeGen.AbstractAsm;

public record AsmFunctionEpilog(string Name) : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString() => "    .cfi_endproc\n";
}