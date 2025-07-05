using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen;

public class Pass3FixupInstructions(RuntimeState options)
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
                            // AF.Newline(),
                            AF.Comment($"Fix {i}"),
                            ..newInstructions,
                            AF.Comment($"end fix for {i}"),
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
        
        // Cmp(imm1, Stack(x)) => Mov(imm1, SCRATCH), Mov(Stack(x), SCRATCH2), Cmp(SCRATCH, SCRATCH2)
        i => i is AsmCmp cmp && cmp.Src1 is AsmImmOperand imm1 && cmp.Src2 is AsmStackOperand stackX
                ? [
                    AF.Mov(imm1, AF.SCRATCH1),
                    AF.LoadStack(stackX, AF.SCRATCH2),
                    AF.Cmp(AF.SCRATCH1, AF.SCRATCH2),
                ] : null,

        // Cmp(Stack(x), imm2) when imm2 >= 4096 => LoadStack(Stack(x), SCRATCH), Mov(imm2, SCRATCH2), Cmp(SCRATCH, imm2)
        i => i is AsmCmp cmp && cmp.Src1 is AsmStackOperand stackX && cmp.Src2 is AsmImmOperand imm2 && imm2.Imm >= 4096
                ? [
                    AF.Mov(imm2, AF.SCRATCH2),
                    AF.LoadStack(stackX, AF.SCRATCH1),
                    AF.Cmp(AF.SCRATCH1, AF.SCRATCH2),
                ] : null,

        // Cmp(Stack(x), imm2) => LoadStack(Stack(x), SCRATCH), Cmp(SCRATCH, imm2)
        i => i is AsmCmp cmp && cmp.Src1 is AsmStackOperand stackX && cmp.Src2 is AsmImmOperand imm2
                ? [
                    AF.LoadStack(stackX, AF.SCRATCH1),
                    AF.Cmp(AF.SCRATCH1, imm2),
                ] : null,

        // Cmp(Stack(x), Stack(y)) => LoadStack(Stack(x), SCRATCH), LoadStack(Stack(y), SCRATCH2), Cmp(SCRATCH, SCRATCH2)
        i => i is AsmCmp cmp && cmp.Src1 is AsmStackOperand stackX && cmp.Src2 is AsmStackOperand stackY
                ? [
                    AF.LoadStack(stackX, AF.SCRATCH1),
                    AF.LoadStack(stackY, AF.SCRATCH2),
                    AF.Cmp(AF.SCRATCH1, AF.SCRATCH2),
                ] : null,

        // SetCC(cond, Stack(x)) => LoadStack(Stack(x), SCRATCH), SetCC(cond, SCRATCH)
        i => i is AsmSetCC setcc && setcc.Dst is AsmStackOperand stackX
                ? [
                    AF.SetCC(setcc.CondCode, AF.SCRATCH1),
                    AF.StoreStack(AF.SCRATCH1, stackX)
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