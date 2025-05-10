using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen.AbstractAsm;

public record AsmPseudoOperand(string Name) : AsmOperand
{
    public AsmPseudoOperand(TacVar v) : this(v.Name) { }

    public override string EmitString() => $"Pseudo(\"{Name}\")";
}