using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmCall(string Name, AsmDestOperand Dst) : AsmInstruction
{
    public Dictionary<string, int> StackOffsets { get; } = [];

    internal int LocalsSize => (int)Math.Ceiling(StackOffsets.Count * 4 / 16.0f) * 16;

    public override string EmitIrString() => $"Call({Name}, {Dst.EmitIrString()})";

    public override string EmitArmString() => $"        bl      _{Name}";

    public override int OperandCount => 0;

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}