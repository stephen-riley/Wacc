namespace Wacc.CodeGen.AbstractAsm;

public interface IAbstractAsm
{
    public void Emit(TextWriter stream);

    public string EmitString();
}