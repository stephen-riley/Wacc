using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options => opts;

    internal List<AsmInstruction> Asm = [];

    public bool Execute()
    {
        TranslateProgram(Options.Tacky);
        Dump("asm IR, pass 1 (codegen)");

        Asm = ResolvePseudoRegistersP2.Execute(Asm);
        Dump("asm IR, pass 2 (resolve tmp vars)");

        Asm = new Pass3FixupInstructionsP3(Options).Execute(Asm);
        Dump("asm IR, pass 3 (fix up addr modes)");

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

        Asm.Add(AF.ProgramEpilogue());
    }

    internal void TranslateFunction(TacFunction f)
    {
        var af = AF.Function(f.Name);

        int curOffset = -8;  // TODO: this isn't right--check on the calling convention in the AArch64 book
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
        Asm.Add(AF.FunctionEpilogue(f.Name, af));
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

            case TacBinary b:
                Asm.Add(AF.Mov(TranslateVal(b.Src1), AF.SCRATCH2));
                Asm.Add(AF.Mov(TranslateVal(b.Src2), AF.PseudoOperand(b.Dst)));
                Asm.Add(b.Op switch
                {
                    "+" => AF.Add(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                    "-" => AF.Subtract(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                    "*" => AF.Mul(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                    "/" => AF.Div(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                    "%" => AF.Mod(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                    _ => throw new InvalidOperationException()
                });
                break;

            case TacReturn r:
                Asm.Add(AF.Mov(TranslateVal(r.Val), AF.RegOperand(ArmReg.RETVAL)));
                Asm.Add(AF.Ret(curFunc.Name));
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

    internal static ArmReg AssignRegisterForTmp(string tmp, int baseRegister = 10)
    {
        if (!tmp.StartsWith("tmp."))
        {
            throw new CodeGenError($"{nameof(AssignRegisterForTmp)} can only handle tmp vars, not {tmp}");
        }

        var tmpIndex = int.Parse(tmp[4..]);
        var withOffset = (ArmReg)tmpIndex + baseRegister;

        return tmpIndex switch
        {
            < 0 => throw new CodeGenError($"{nameof(AssignRegisterForTmp)}: {tmp} is invalid"),
            _ when withOffset > ArmReg.LAST_REGISTER => throw new CodeGenError($"{nameof(AssignRegisterForTmp)}: {tmp} is invalid"),
            _ => (ArmReg)tmpIndex + baseRegister
        };
    }
}