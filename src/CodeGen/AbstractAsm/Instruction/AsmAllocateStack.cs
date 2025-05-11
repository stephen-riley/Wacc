namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmAllocateStack(int Size) : AsmInstruction
{
    public override string EmitIrString() => $"AllocateStack({Size})";

    public override string EmitArmString() =>
$"""
        sub     sp, sp, #{Size}         ; allocate stack frame
        stp     fp, lr, [sp, #16]       ; save FP and LR
        add     fp, sp, #16             ; set FP for new frame
        
""";
}