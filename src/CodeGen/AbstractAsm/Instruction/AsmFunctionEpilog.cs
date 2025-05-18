using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmFunctionEpilogue(string Name, AsmFunction Func) : AsmInstruction
{
    public override string EmitIrString() => $"FuncEpilog({Name})";

    public override string EmitArmString() => string.Join("\n",
        $"",
        $"        ldp     fp, lr, [sp, #16]",
        $"        add     sp, sp, #{Func.LocalsSize}",
        $"        .cfi_endproc",
        $"        ; end function _{Name}"
    );

    public override int OperandCount => 0;

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}