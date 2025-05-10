
namespace Wacc.CodeGen.AbstractAsm;

public record AsmProgram() : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString() =>
        "    .section __TEXT,__text,regular,pure_instructions\n" +
        "    .align 2";

    public static void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "";
}