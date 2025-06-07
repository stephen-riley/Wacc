using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;

namespace Wacc.CodeGen.AbstractAsm.Instruction;

public record AsmProgram(string Filename) : AsmInstruction
{
    public static void EmitEpilog(TextWriter stream) => stream.WriteLine(EmitEpilogString());

    public static string EmitEpilogString() => "";

    public override string EmitIrString() => $"Program(\"{Filename}\")";

    public override string EmitArmString() => string.Join("\n",
       $"        ; compiled from {Filename}",
        "",
        "        .section __TEXT,__text,regular,pure_instructions",
        "        .align 2",
        "",
        "        ; register aliases",
        "        retval    .req w0",
        "        scratch1  .req w9",
        "        scratch2  .req w10",
        "        scratch3  .req w11",
        "        fp        .req w29",
        "        lr        .req w30",
        ""
    );

    public override int OperandCount => 0;

    public override AsmOperand? GetOperand(int n) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");

    public override AsmInstruction SetOperand(int n, AsmOperand o) => throw new CodeGenError($"{GetType().Name} only has {OperandCount} operands");
}