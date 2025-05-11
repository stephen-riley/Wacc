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
            pass2.Add(i switch
            {
                AsmBitNot bn => AF.BitNot(Src(bn.Src)),
                AsmFunction f => Ext.Do(() => { curFunc = f; return f; }),
                AsmMov m => AF.Mov(Src(m.Src), Dst(m.Dst)),
                AsmNeg n => AF.Neg(Src(n.Src)),
                _ => i
            });
        }

        return pass2;
    }
}