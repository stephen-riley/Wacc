
namespace Wacc.CodeGen.AbstractAsm;

public record AsmProgram(string Filename) : IAbstractAsm
{
    public string EmitString() =>
        $"    ; compiled from {Filename}\n" +
        "\n" +
        "    .section __TEXT,__text,regular,pure_instructions\n" +
        "    .align 2";

    public static void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "";
}