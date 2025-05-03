using System.Reflection;
using System.Text;

namespace Wacc.CodeGen.AbstractAsm;

public record FunctionGen(string Name, IEnumerable<IAbstractAsm> Body) : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString()
    {
        var sb = new StringBuilder();
        sb.Append($"    .globl _{Name}\n");
        sb.Append($"_{Name}:\n");
        foreach (var a in Body)
        {
            sb.Append("    ").Append(a.EmitString()).Append('\n');
        }
        return sb.ToString();
    }
}