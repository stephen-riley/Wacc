using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmFunctionEpilogue(string Name, AsmFunction Func) : AsmInstruction
{
    public override string EmitIrString() => $"FuncEpilog({Name})";

    public override string EmitArmString() => string.Join("\n",
        "",
        "        .cfi_endproc",
        $"        ; end function _{Name}"
    );

    public override int OperandCount => 0;

    public override AsmOperand? Operand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}