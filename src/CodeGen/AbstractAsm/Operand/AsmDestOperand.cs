namespace Wacc.CodeGen.AbstractAsm.Operand;

public abstract record AsmDestOperand() : AsmOperand
{
    public override void Emit(TextWriter stream)
        => throw new NotImplementedException();

    public override string EmitString()
        => throw new NotImplementedException();
}