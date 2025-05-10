
namespace Wacc.AbstractAsm;

public record ProgramEpilogGen() : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    // TODO: might need this in the future
    public string EmitString() => "";
}