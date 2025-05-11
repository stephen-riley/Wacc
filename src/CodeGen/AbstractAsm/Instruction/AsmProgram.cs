namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmProgram(string Filename) : AsmInstruction
{
    public override string EmitIrString() =>
        $"        ; compiled from {Filename}\n" +
        "\n" +
        "        .section __TEXT,__text,regular,pure_instructions\n" +
        "        .align 2\n" +
        "scratch .req W9\n" +
        "retval  .req W0\n";

    public static void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "";
}