using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmMov(AsmOperand Src, AsmDestOperand Dst) : AsmInstruction
{
    public override int OperandCount => 2;

    public override string EmitArmString() => Src switch
    {
        _ when Src is AsmImmOperand imm && imm.Imm < 65536 => $"        mov     {Dst.EmitArmString()}, #{imm.Imm}",
        _ when Src is AsmImmOperand imm => $"        ldr     {Dst.EmitArmString()}, ={imm.Imm}",
        _ => $"        mov     {Dst.EmitArmString()}, {Src.EmitArmString()}"
    };

    public override string EmitIrString() => $"Mov({Src.EmitIrString()}, {Dst.EmitIrString()})";

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
            2 => this with { Dst = (AsmDestOperand)o },
            _ => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands")
        };
}