using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen;

public class Pass3FixupInstructionsP3(RuntimeState options)
{
    internal RuntimeState Options => options;

    public List<AsmInstruction> Execute(List<AsmInstruction> Asm)
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
                    if (Options.Verbose)
                    {
                        pass3.AddRange([
                            AF.Newline(),
                            AF.Comment($"Fixup on {i}"),
                            ..newInstructions,
                            AF.Comment(),
                        ]);
                    }
                    else
                    {
                        pass3.AddRange(newInstructions);
                    }

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
                    AF.LoadStack(stackX, AF.SCRATCH1),
                    AF.StoreStack(AF.SCRATCH1, stackY),
                  ] : null,

        // Mov(Constant, Stack(x)) => Mov(Constant, SCRATCH), StoreStack(SCRATCH, Stack(x))
        i => i is AsmMov mov && mov.Src is AsmImmOperand imm && mov.Dst is AsmStackOperand stackY
                ? [
                    AF.Mov(imm, AF.SCRATCH1),
                    AF.StoreStack(AF.SCRATCH1, stackY),
                ] : null,

        // Mov(Stack(x), Reg(y)) => LoadStack(Stack(x), Reg(y))
        i => i is AsmMov mov && mov.Src is AsmStackOperand stackX && mov.Dst is AsmRegOperand regY
                ? [
                    AF.LoadStack(stackX, regY),
                ] : null,

        // Cmp(imm1, imm2) => Mov(imm1, SCRATCH), Cmp(SCRATCH, imm2)
        i => i is AsmCmp cmp && cmp.Src1 is AsmImmOperand imm1 && cmp.Src2 is AsmImmOperand imm2
                ? [
                    AF.Mov(imm1, AF.SCRATCH1),
                    AF.Cmp(AF.SCRATCH1, imm2),
                ] : null,

        // other Src-only instructions (eg. Unary):
        // op(Stack(x)) => LoadStack(Stack(x), SCRATCH), op(SCRATCH), StoreStack(SCRATCH, Stack(x))
        i => i.OperandCount == 1 && i.GetOperand(1) is AsmStackOperand stackX
                ? [
                    AF.LoadStack(stackX, AF.SCRATCH1),
                    AF.Create(i.GetType(), AF.SCRATCH1),
                    AF.StoreStack(AF.SCRATCH1, stackX),
                ] : null,
        
        // three-address instructions
        i => {
                if(i.OperandCount == 3 && i.Operands.OfType<AsmStackOperand>().Any()) {
                    var stackOp = i.Operands.OfType<AsmStackOperand>().First();
                    return  [
                        AF.LoadStack(stackOp, AF.SCRATCH1),
                        AF.Create(i.GetType(), AF.SCRATCH2, AF.SCRATCH1, AF.SCRATCH1),
                        AF.StoreStack(AF.SCRATCH1, stackOp),
                    ];
                } else {
                    return null;
                }
            }
    ];
}