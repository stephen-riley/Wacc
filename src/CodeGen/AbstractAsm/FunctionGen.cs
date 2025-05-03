using System.Reflection;
using System.Text;

namespace Wacc.CodeGen.AbstractAsm;

public record FunctionGen(string Name, IEnumerable<IAbstractAsm> Body) : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    .globl _{Name}");
        sb.AppendLine($"_{Name}:");
        sb.AppendLine("    .cfi_startproc");
        foreach (var a in Body)
        {
            sb.Append("    ").AppendLine(a.EmitString());
        }
        sb.AppendLine("    .cfi_endproc");
        return sb.ToString();
    }
}