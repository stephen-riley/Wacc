using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;

namespace Wacc.CodeGen;

public static class Pass3FixupInstructionsP3
{
    public static List<AsmInstruction> Execute(List<AsmInstruction> Asm)
    {
        var pass3 = new List<AsmInstruction>();

        foreach (var i in Asm)
        {
            if (i is AsmMov mov && mov.Src is AsmStackOperand srcOpd && mov.Dst is AsmStackOperand dstOpd)
            {
                // Replace Mov(Stack(x), Stack(y)) with
                //   LoadStack(Stack(x), Reg(SCRATCH))
                //   StoreStack(Reg(SCRATCH), Stack(y))
                pass3.AddRange([
                    AF.LoadStack(srcOpd, AF.RegOperand(Register.SCRATCH)),
                    AF.StoreStack(AF.RegOperand(Register.SCRATCH), dstOpd)
                ]);
            }
            else
            {
                pass3.Add(i);
            }
        }

        return pass3;
    }
}