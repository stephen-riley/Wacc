using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using static Wacc.CodeGen.AbstractAsm.Register;

namespace Wacc.CodeGen;

public static class Pass3FixupInstructionsP3
{
    public static List<AsmInstruction> Execute(List<AsmInstruction> Asm)
    {
        var pass3 = new List<AsmInstruction>();

        foreach (var i in Asm)
        {
            var processed = false;

            foreach (var rule in Rules)
            {
                var newInstructions = rule(i);
                if (newInstructions is not null)
                {
                    pass3.AddRange(newInstructions);
                    processed = true;
                    goto CONT;
                }
            }
        CONT:
            if (!processed)
            {
                pass3.Add(i);
            }
        }

        return pass3;
    }

    internal static List<Func<AsmInstruction, IEnumerable<AsmInstruction>?>> Rules = [

        // Mov(Stack(x), Stack(y)) => LoadStack(Stack(x), SCRATCH), StoreStack(SCRATCH, Stack(y))
        i => i is AsmMov mov && mov.Src is AsmStackOperand stackX && mov.Dst is AsmStackOperand stackY
                ? [
                    AF.Comment($"Fixup on {mov}"),
                    AF.LoadStack(stackX, AF.RegOperand(SCRATCH)),
                    AF.StoreStack(AF.RegOperand(SCRATCH), stackY)
                  ] : null,

        // Mov(Constant, Stack(x)) => Mov(Constant, SCRATCH), StoreStack(SCRATCH, Stack(x))
        i => i is AsmMov mov && mov.Src is AsmImmOperand imm && mov.Dst is AsmStackOperand stackY
                ? [
                    AF.Comment($"Fixup on {mov}"),
                    AF.Mov(imm,AF.RegOperand(SCRATCH)),
                    AF.StoreStack(AF.RegOperand(SCRATCH), stackY)
                ] : null,

        // Mov(Stack(x), Reg(y)) => LoadStack(Stack(x), Reg(y))
        i => i is AsmMov mov && mov.Src is AsmStackOperand stackX && mov.Dst is AsmRegOperand regY
                ? [
                    AF.Comment($"Fixup on {mov}"),
                    AF.LoadStack(stackX, regY)
                ] : null,

        // other Src-only instructions (eg. Unary):
        // op(Stack(x)) => LoadStack(Stack(x), SCRATCH), op(SCRATCH), StoreStack(SCRATCH, Stack(x))
        i => i.OperandCount == 1 && i.Operand(1) is AsmStackOperand stackX
                ? [
                    AF.Comment($"Fixup on {i}"),
                    AF.LoadStack(stackX, AF.RegOperand(SCRATCH)),
                    AF.Create(i.GetType(), AF.RegOperand(SCRATCH)),
                    AF.StoreStack(AF.RegOperand(SCRATCH), stackX)
                ] : null
    ];
}