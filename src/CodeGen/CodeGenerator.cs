using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options => opts;

    internal List<AsmInstruction> Asm = [];

    public bool Execute()
    {
        TranslateProgram(Options.Tacky);
        Dump("asm IR, pass 1");

        Asm = ResolvePseudoRegisters();
        Dump("asm IR, pass 2");

        Asm = FixUpInstructions();
        Dump("asm IR, pass 3");

        Options.AbstractAsm = Asm;

        return true;
    }

    internal void Dump(string title)
    {
        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"{title.ToUpper()}:");
            Console.Error.WriteLine(new string('=', title.Length + 1));
        }

        if (Options.Verbose || Options.OnlyThroughCodeGen)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;

            foreach (var a in Asm)
            {
                a?.EmitIr(stream);
            }
        }
    }

    internal void TranslateProgram(TacProgram p)
    {
        Asm.Add(AF.Program(Options.InputFile));

        foreach (var f in p.Functions)
        {
            TranslateFunction(f);
        }

        Asm.Add(AF.ProgramEpilog());
    }

    internal void TranslateFunction(TacFunction f)
    {
        var af = AF.Function(f.Name);

        int curOffset = 0;  // TODO: this isn't right--check on the calling convention in the AArch64 book
        foreach (var l in f.Locals)
        {
            af.StackOffsets[l.Name] = curOffset;
            curOffset -= 4;
        }

        Asm.Add(af);
        Asm.Add(AF.AllocateStack(af.LocalsSize));
        foreach (var i in f.Instructions)
        {
            TranslateTacInstruction(i, af);
        }
        Asm.Add(AF.FunctionEpilog(f.Name));
    }

    internal void TranslateTacInstruction(ITackyInstr instr, AsmFunction curFunc)
    {
        var bundle = new List<AsmInstruction>();

        switch (instr)
        {
            case TacUnary u when u.OpName == "Negate":
                Asm.Add(AF.Mov(TranslateVal(u.Src), AF.PseudoOperand(u.Dst)));
                Asm.Add(AF.Neg(AF.PseudoOperand(u.Dst)));
                break;

            case TacUnary u when u.OpName == "Complement":
                Asm.Add(AF.Mov(TranslateVal(u.Src), AF.PseudoOperand(u.Dst)));
                Asm.Add(AF.BitNot(AF.PseudoOperand(u.Dst)));
                break;

            case TacReturn r:
                Asm.Add(AF.Mov(TranslateVal(r.Val), AF.RegOperand(Register.RETVAL)));
                Asm.Add(AF.Ret());
                break;

            default:
                throw new CodeGenError($"{nameof(CodeGenerator)}.{nameof(TranslateTacInstruction)} can't/shouldn't handle type {instr.GetType().Name} yet");
        }
    }

    internal AsmOperand TranslateVal(TacVal val)
        => val switch
        {
            TacConstant c => AF.ImmOperand(c.Value),
            TacVar v => AF.PseudoOperand(v.Name),
            _ => throw new CodeGenError($"{nameof(TranslateVal)}: can't handle TacVal type {val.GetType().Name}")
        };

    internal static Register AssignRegisterForTmp(string tmp, int baseRegister = 10)
    {
        if (!tmp.StartsWith("tmp."))
        {
            throw new CodeGenError($"{nameof(AssignRegisterForTmp)} can only handle tmp vars, not {tmp}");
        }

        var tmpIndex = int.Parse(tmp[4..]);
        var withOffset = (Register)tmpIndex + baseRegister;

        return tmpIndex switch
        {
            < 0 => throw new CodeGenError($"{nameof(AssignRegisterForTmp)}: {tmp} is invalid"),
            _ when withOffset > Register.LAST_REGISTER => throw new CodeGenError($"{nameof(AssignRegisterForTmp)}: {tmp} is invalid"),
            _ => (Register)tmpIndex + baseRegister
        };
    }

    internal List<AsmInstruction> ResolvePseudoRegisters()
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

    internal List<AsmInstruction> FixUpInstructions()
    {
        var pass3 = new List<AsmInstruction>();

        foreach (var i in Asm)
        {
            if (i is AsmMov mov && mov.Src is AsmStackOperand && mov.Dst is AsmStackOperand)
            {
                // Replace Mov(Stack(x), Stack(y)) with
                //   LoadStack(Stack(x), Reg(SCRATCH))
                //   StoreStack(Reg(SCRATCH), Stack(y))
                pass3.AddRange([
                    AF.LoadStack((AsmStackOperand)mov.Src, AF.RegOperand(Register.SCRATCH)),
                    AF.StoreStack(AF.RegOperand(Register.SCRATCH), (AsmStackOperand)mov.Dst)
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