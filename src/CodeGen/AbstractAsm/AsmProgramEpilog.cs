
namespace Wacc.CodeGen.AbstractAsm;

public record AsmProgramEpilog() : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    // TODO: might need this in the future
    public string EmitString() => "";
}