using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen.AbstractAsm;

public static class AF
{
    public static AsmAdd Add(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmAllocateStack AllocateStack(int Size) => new(Size);
    public static AsmBitNot BitNot(AsmOperand Src) => new(Src);
    public static AsmBitwiseAnd BitwiseAnd(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmBitwiseLeft BitwiseLeft(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmBitwiseOr BitwiseOr(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmBitwiseRight BitwiseRight(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmBitwiseXor BitwiseXor(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmCmp Cmp(AsmOperand Src1, AsmOperand Src2) => new(Src1, Src2);
    public static AsmComment Comment(string Comment = "") => new(Comment);
    public static AsmDiv Div(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmFunction Function(string Name) => new(Name);
    public static AsmFunctionEpilogue FunctionEpilogue(string Name, AsmFunction Func) => new(Name, Func);
    public static AsmJmp Jmp(string Label) => new(Label);
    public static AsmJmpCC JmpCC(AsmCmp.CondCode CondCode, string Label) => new(CondCode, Label);
    public static AsmLabel Label(string Label) => new(Label);
    public static AsmLoadStack LoadStack(AsmStackOperand Src, AsmDestOperand Dst) => new(Src, Dst);
    public static AsmMod Mod(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmMov Mov(AsmOperand Src, AsmDestOperand Dst) => new(Src, Dst);
    public static AsmMul Mul(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmNeg Neg(AsmOperand Src) => new(Src);
    public static AsmNewline Newline() => new();
    public static AsmProgram Program(string Filename) => new(Filename);
    public static AsmProgramEpilogue ProgramEpilogue() => new();
    public static AsmRet Ret(string FuncName) => new(FuncName);
    public static AsmSetCC SetCC(AsmCmp.CondCode CondCode, AsmOperand Src) => new(CondCode, Src);
    public static AsmStoreStack StoreStack(AsmOperand Src, AsmStackOperand Dst) => new(Src, Dst);
    public static AsmSubtract Subtract(AsmOperand Src1, AsmOperand Src2, AsmDestOperand Dst) => new(Src1, Src2, Dst);
    public static AsmImmOperand ImmOperand(int Imm) => new(Imm);
    public static AsmPseudoOperand PseudoOperand(string Name) => new(Name);
    public static AsmPseudoOperand PseudoOperand(TacVar v) => new(v);
    public static AsmRegOperand RegOperand(ArmReg Reg) => new(Reg);
    public static AsmStackOperand StackOperand(int Offset) => new(Offset);

    public static AsmInstruction Create(Type type, AsmOperand src1, AsmOperand src2, AsmOperand dst)
    {
        var i = Activator.CreateInstance(type, src1, src2, dst) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmInstruction Create(Type type, AsmOperand src, AsmOperand dst)
    {
        var i = Activator.CreateInstance(type, src, dst) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmInstruction Create(Type type, AsmOperand src)
    {
        var i = Activator.CreateInstance(type, src) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmInstruction Create(Type type)
    {
        var i = Activator.CreateInstance(type) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmRegOperand FP => new(ArmReg.FP);

    public static AsmRegOperand SP => new(ArmReg.SP);

    public static AsmRegOperand SCRATCH1 => new(ArmReg.SCRATCH1);

    public static AsmRegOperand SCRATCH2 => new(ArmReg.SCRATCH2);

    public static AsmRegOperand SCRATCH3 => new(ArmReg.SCRATCH3);

    public static AsmRegOperand RETVAL => new(ArmReg.RETVAL);
}

