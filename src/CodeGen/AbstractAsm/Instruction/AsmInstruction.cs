using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public abstract record AsmInstruction : AsmObject
{
    public abstract int OperandCount { get; }

    public abstract AsmOperand? Operand(int n);
}