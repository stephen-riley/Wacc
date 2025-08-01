using Wacc.Ast;
using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;
using static Wacc.Tokens.TokenType;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options => opts;

    internal List<AsmInstruction> Asm = [];

    public bool Execute()
    {
        TranslateProgram(Options.Tacky);
        Dump("asm IR, pass 1 (codegen)", !Options.Silent);

        Asm = Pass2ResolvePseudoRegisters.Execute(Asm);
        Dump("asm IR, pass 2 (resolve tmp vars)", !Options.Silent);

        Asm = new Pass3FixupInstructions(Options).Execute(Asm);
        Dump("asm IR, pass 3 (fix up addr modes)", !Options.Silent);

        Options.AbstractAsm = Asm;

        return true;
    }

    internal void Dump(string title, bool show = true)
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
        Asm.Add(AF.Newline());

        foreach (var i in f.Instructions)
        {
            TranslateTacInstruction(i, af);
        }
        Asm.Add(AF.FunctionEpilogue(f.Name, af));
    }

    internal void TranslateTacInstruction(ITackyInstr instr, AsmFunction curFunc)
    {
        Asm.Add(AF.InstructionComment(instr.ToString()));

        switch (instr)
        {
            case TacJump j:
                Asm.Add(AF.Jmp(j.Identifier));
                break;

            case TacJumpIfZero jz:
                Asm.AddRange([
                    AF.Mov(TranslateVal(jz.Src), AF.SCRATCH1),
                    AF.Cmp(AF.ZR, AF.SCRATCH1),
                    AF.JmpCC(AsmCmp.CondCode.EQ, jz.Identifier)
                ]);
                break;

            case TacJumpIfNotZero jnz:
                Asm.AddRange([
                    AF.Mov(TranslateVal(jnz.Src), AF.SCRATCH1),
                    AF.Cmp(AF.ZR, AF.SCRATCH1),
                    AF.JmpCC(AsmCmp.CondCode.NE, jnz.Identifier)
                ]);
                break;

            case TacUnary u when u.OpName == "Negate":
                Asm.Add(AF.Mov(TranslateVal(u.Src), AF.PseudoOperand(u.Dst)));
                Asm.Add(AF.Neg(AF.PseudoOperand(u.Dst)));
                break;

            case TacUnary u when u.OpName == "Complement":
                Asm.Add(AF.Mov(TranslateVal(u.Src), AF.PseudoOperand(u.Dst)));
                Asm.Add(AF.BitNot(AF.PseudoOperand(u.Dst)));
                break;

            case TacUnary u when u.OpName == "Not":
                Asm.AddRange([
                    AF.Mov(TranslateVal(u.Src), AF.SCRATCH1),
                    AF.Cmp(AF.ZR, AF.SCRATCH1),
                    AF.SetCC(AsmCmp.CondCode.EQ, AF.PseudoOperand(u.Dst))
                ]);
                break;

            case TacBinary b when BinaryOp.RelationalOps.Contains(b.Op):
                Asm.AddRange([
                    AF.Cmp(TranslateVal(b.Src1), TranslateVal(b.Src2)),
                    AF.SetCC(TacBinary.CondCode[b.Op], AF.PseudoOperand(b.Dst))
                ]);
                break;

            case TacBinary b:
                Asm.AddRange([
                    AF.Mov(TranslateVal(b.Src1), AF.SCRATCH2),
                    AF.Mov(TranslateVal(b.Src2), AF.PseudoOperand(b.Dst)),
                    b.Op switch
                    {
                        Plus => AF.Add(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        Minus => AF.Subtract(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        Asterisk => AF.Mul(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        Div => AF.Div(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        Mod => AF.Mod(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        BitwiseAnd => AF.BitwiseAnd(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        BitwiseLeft => AF.BitwiseLeft(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        BitwiseOr => AF.BitwiseOr(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        BitwiseRight => AF.BitwiseRight(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        BitwiseXor => AF.BitwiseXor(AF.SCRATCH2, AF.PseudoOperand(b.Dst), AF.PseudoOperand(b.Dst)),
                        _ => throw new NotImplementedException($"Operator \"{b.Op}\" not (yet) impelemented for TacBinary code generation")
                    }
                ]);
                break;

            case TacReturn r:
                Asm.Add(AF.Mov(TranslateVal(r.Val), AF.RegOperand(ArmReg.RETVAL)));
                Asm.Add(AF.Ret(curFunc.Name));
                break;

            case TacCopy c:
                Asm.Add(AF.Mov(TranslateVal(c.Src), AF.PseudoOperand(c.Dst)));
                break;

            case TacLabel l:
                Asm.Add(AF.Label(l.Identifier));
                break;

            default:
                throw new CodeGenError($"{nameof(CodeGenerator)}.{nameof(TranslateTacInstruction)} can't/shouldn't handle type {instr.GetType().Name} yet");
        }

        if (instr is not TacLabel)
        {
            Asm.Add(AF.Newline());
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