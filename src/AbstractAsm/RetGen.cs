
namespace Wacc.AbstractAsm;

public record RetGen : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.Write(EmitString());

    public string EmitString() => "ret";
}