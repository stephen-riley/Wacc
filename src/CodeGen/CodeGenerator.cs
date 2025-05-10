using Wacc.Ast;
using Wacc.CodeGen.AbstractAsm;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options => opts;

    internal List<IAbstractAsm> Asm = [];

    public bool Execute()
    {
        TranslateProgram(Options.Tacky);
        Options.AbstractAsm = Asm;

        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("ABSTRACT ASM:");
        }

        if (Options.Verbose || Options.OnlyThroughCodeGen)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;

            foreach (var a in Asm)
            {
                a?.Emit(stream);
            }
        }

        return true;
    }

    internal void TranslateProgram(TacProgram p)
    {
        Asm.Add(new AsmProgram(Options.InputFile));

        foreach (var f in p.Functions)
        {
            TranslateFunction(f);
        }

        Asm.Add(new AsmProgramEpilog());
    }

    internal void TranslateFunction(TacFunction f)
    {
        Asm.Add(new AsmFunction(f.Name));
        foreach (var i in f.Instructions)
        {
            TranslateTacInstruction(i);
        }
        Asm.Add(new AsmFunctionEpilog(f.Name));
    }

    internal void TranslateTacInstruction(ITackyInstr instr)
    {
        var bundle = new List<IAbstractAsm>();

        switch (instr)
        {
            case TacUnary u when u.OpName == "Negate":
                Asm.Add(new AsmMov(TranslateVal(u.Src), new AsmPseudoOperand(u.Dst)));
                Asm.Add(new AsmNeg(new AsmPseudoOperand(u.Dst)));
                break;

            case TacUnary u when u.OpName == "Complement":
                Asm.Add(new AsmMov(TranslateVal(u.Src), new AsmPseudoOperand(u.Dst)));
                Asm.Add(new AsmBitNot(new AsmPseudoOperand(u.Dst)));
                break;

            case TacReturn r:
                Asm.Add(new AsmMov(TranslateVal(r.Val), new AsmPseudoOperand(Register.RET.ToString())));
                Asm.Add(new AsmRet());
                break;

            default:
                throw new CodeGenError($"{nameof(CodeGenerator)}.{nameof(TranslateTacInstruction)} can't/shouldn't handle type {instr.GetType().Name} yet");
        }
    }

    internal AsmOperand TranslateVal(TacVal val)
        => val switch
        {
            TacConstant c => new AsmImmOperand(c.Value),
            TacVar v => new AsmPseudoOperand(v.Name),
            _ => throw new CodeGenError($"{nameof(TranslateVal)}: can't handle TacVal type {val.GetType().Name}")
        };

    internal static Register AssignRegisterForTmp(string tmp, int baseRegister = 9)
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
}