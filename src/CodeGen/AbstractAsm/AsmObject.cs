namespace Wacc.CodeGen.AbstractAsm;

public abstract record AsmObject
{
    public abstract string EmitIrString();
    public virtual void EmitIr(TextWriter stream) => stream.WriteLine(EmitIrString());
}