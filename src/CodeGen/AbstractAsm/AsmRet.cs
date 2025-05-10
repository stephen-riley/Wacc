
namespace Wacc.CodeGen.AbstractAsm;

public record AsmRet : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.Write(EmitString());

    public string EmitString() => "ret";
}