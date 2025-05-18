using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Extensions;

namespace Wacc.CodeGen;

public static class ResolvePseudoRegistersP2
{
    public static List<AsmInstruction> Execute(List<AsmInstruction> Asm)
    {
        AsmFunction curFunc = null!;
        var pass2 = new List<AsmInstruction>();

        AsmOperand Src(AsmOperand o)
            => o is AsmPseudoOperand po ? AF.StackOperand(curFunc.StackOffsets[po.Name]) : o;

        AsmDestOperand Dst(AsmDestOperand d)
            => d is AsmPseudoOperand po ? AF.StackOperand(curFunc.StackOffsets[po.Name]) : d;

        foreach (var i in Asm)
        {
            if (i is AsmFunction f)
            {
                curFunc = f;
                continue;
            }

            var newInstr = i;

            for (var opIndex = 1; opIndex <= newInstr.OperandCount; opIndex++)
            {
                if (newInstr.TryGetOperand(opIndex, out var operand))
                {
                    var newOperand = operand is AsmDestOperand ado ? Dst(ado) : Src(operand);
                    newInstr = newInstr.SetOperand(opIndex, newOperand);
                }
            }

            pass2.Add(newInstr);
            // pass2.Add(i switch
            // {
            //     AsmBitNot bn => AF.BitNot(Src(bn.Src)),
            //     AsmFunction f => Ext.Do(() => { curFunc = f; return f; }),
            //     AsmMov m => AF.Mov(Src(m.Src), Dst(m.Dst)),
            //     AsmNeg n => AF.Neg(Src(n.Src)),
            //     _ => i
            // });
        }

        return pass2;
    }
}