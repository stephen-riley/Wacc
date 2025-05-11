namespace Wacc.CodeGen.AbstractAsm.Operand;

using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

public record AsmPseudoOperand(string Name) : AsmDestOperand
{
    public AsmPseudoOperand(TacVar v) : this(v.Name) { }

    public override string EmitArmString() => throw new CodeGenError("should not be any pseudo-register operands in Arm output");

    public override string EmitIrString() => $"Pseudo(\"{Name}\")";
}