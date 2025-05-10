using System.Text;
using Wacc.Tacky.Instruction;

namespace Wacc.AbstractAsm;

public record FunctionEpilogGen(string Name) : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString() => "    .cfi_endproc\n";
}