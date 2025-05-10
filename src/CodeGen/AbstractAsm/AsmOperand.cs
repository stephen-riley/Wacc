
namespace Wacc.CodeGen.AbstractAsm;

public record AsmOperand : IAbstractAsm
{
    public virtual void Emit(TextWriter stream)
        => throw new NotImplementedException();

    public virtual string EmitString()
        => throw new NotImplementedException();
}