using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmStoreStack(AsmOperand Src, AsmStackOperand Dst) : AsmInstruction
{
    public override int OperandCount => 2;

    public override string EmitIrString() => $"StoreStack({Src.EmitIrString()}, {Dst.EmitIrString()})";

    public override string EmitArmString() => $"        str     {Src.EmitArmString()}, {Dst.EmitArmString()}";

    public override AsmOperand? GetOperand(int n)
    => n switch
    {
        1 => Src,
        2 => Dst,
        _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
    };

    public override AsmInstruction SetOperand(int n, AsmOperand o)
        => n switch
        {
            1 => this with { Src = o },
            3 => this with { Dst = (AsmStackOperand)o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}