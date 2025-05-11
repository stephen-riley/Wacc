using System.Text;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmFunction(string Name) : AsmInstruction
{
    public Dictionary<string, int> StackOffsets { get; } = [];

    internal int LocalsSize => (int)Math.Ceiling(StackOffsets.Count * 4 / 16.0f) * 16;

    public override string EmitIrString()
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"        .globl _{Name}");
        sb.AppendLine($"_{Name}:");
        sb.AppendLine("        .cfi_startproc");
        return sb.ToString();
    }

    public static void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "        .cfi_endproc\n";
}