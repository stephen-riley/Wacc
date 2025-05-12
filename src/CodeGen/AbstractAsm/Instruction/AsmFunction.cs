using System.Text;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmFunction(string Name) : AsmInstruction
{
    public Dictionary<string, int> StackOffsets { get; } = [];

    internal int LocalsSize => (int)Math.Ceiling(StackOffsets.Count * 4 / 16.0f) * 16;

    public override string EmitIrString() => $"Function({Name})";

    public override string EmitArmString()
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"        .globl _{Name}");
        sb.AppendLine($"_{Name}:");
        sb.AppendLine("        .cfi_startproc");
        return sb.ToString();
    }

    public override int OperandCount => 0;

    public override AsmOperand? Operand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}