using System.Text;
using Wacc.Tacky.Instruction;

namespace Wacc.AbstractAsm;

public record FunctionGen(string Name, IEnumerable<ITackyInstr> Body) : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());
    public string EmitString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    .globl _{Name}");
        sb.AppendLine($"_{Name}:");
        sb.AppendLine("    .cfi_startproc");
        return sb.ToString();
    }

    public void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "    .cfi_endproc\n";
}