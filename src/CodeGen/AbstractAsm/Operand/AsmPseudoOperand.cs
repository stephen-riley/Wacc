namespace Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Tacky.Instruction;

public record AsmPseudoOperand(string Name) : AsmDestOperand
{
    public AsmPseudoOperand(TacVar v) : this(v.Name) { }

    public override string EmitString() => $"Pseudo(\"{Name}\")";
}