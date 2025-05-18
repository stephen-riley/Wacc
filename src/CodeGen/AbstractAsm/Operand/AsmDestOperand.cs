namespace Wacc.CodeGen.AbstractAsm.Operand;

public abstract record AsmDestOperand() : AsmOperand
{
    public override void EmitIr(TextWriter stream)
        => throw new NotImplementedException($"{GetType().Name}.{nameof(EmitIr)}");

    public override string EmitIrString()
        => throw new NotImplementedException($"{GetType().Name}.{nameof(EmitIrString)}");
}