using System.Diagnostics.CodeAnalysis;
using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public abstract record AsmInstruction : AsmObject
{
    public abstract int OperandCount { get; }

    public abstract AsmOperand? GetOperand(int n);

    public virtual bool TryGetOperand(int n, [NotNullWhen(true)] out AsmOperand? operandOut)
    {
        operandOut = GetOperand(n);
        return operandOut is not null;
    }

    public abstract AsmInstruction SetOperand(int n, AsmOperand o);

    public virtual IEnumerable<AsmOperand> Operands => Enumerable.Range(1, OperandCount).Select(n => GetOperand(n) ?? throw new InvalidOperationException());
}