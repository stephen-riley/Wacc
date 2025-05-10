using System.Text;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen.AbstractAsm;

public record AsmFunction(string Name) : IAbstractAsm
{
    public string EmitString()
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"    .globl _{Name}");
        sb.AppendLine($"_{Name}:");
        sb.AppendLine("    .cfi_startproc");
        return sb.ToString();
    }

    public void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "    .cfi_endproc\n";
}