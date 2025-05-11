namespace Wacc.CodeGen.AbstractAsm;

public abstract record AsmObject
{
    public abstract string EmitString();
    public virtual void Emit(TextWriter stream) => stream.WriteLine(EmitString());
}