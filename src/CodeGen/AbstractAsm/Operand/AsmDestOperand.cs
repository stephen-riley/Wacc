namespace Wacc.CodeGen.AbstractAsm.Operand;

public abstract record AsmDestOperand() : AsmOperand
{
    public override void EmitIr(TextWriter stream)
        => throw new NotImplementedException();

    public override string EmitIrString()
        => throw new NotImplementedException();
}