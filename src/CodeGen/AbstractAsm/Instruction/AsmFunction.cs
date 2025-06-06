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
        sb.AppendLine($"        .global _{Name}");
        sb.AppendLine($"_{Name}:");
        sb.AppendLine("        .cfi_startproc");
        return sb.ToString();
    }

    public override int OperandCount => 0;

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}