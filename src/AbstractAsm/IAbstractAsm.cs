namespace Wacc.AbstractAsm;

public interface IAbstractAsm
{
    public void Emit(TextWriter stream);

    public string EmitString();
}