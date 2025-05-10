namespace Wacc.CodeGen.AbstractAsm;

public interface IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString();
}