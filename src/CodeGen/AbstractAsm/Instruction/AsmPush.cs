using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmPush(AsmOperand Src) : AsmInstruction
{
    public override int OperandCount => 1;

    public override string EmitArmString() => $"        str     {Src.EmitArmString()}, [sp, #-16]!";

    public override string EmitIrString() => $"Push({Src.EmitIrString()})";

    public override AsmOperand? GetOperand(int n)
        => n switch
        {
            1 => Src,
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => n switch
        {
            1 => this with { Src = (AsmDestOperand)o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}