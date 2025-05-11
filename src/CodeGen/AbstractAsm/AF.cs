using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen.AbstractAsm;

public static class AF
{
    public static AsmAllocateStack AllocateStack(int Size) => new(Size);
    public static AsmBitNot BitNot(AsmOperand Src) => new(Src);
    public static AsmFunction Function(string Name) => new(Name);
    public static AsmFunctionEpilog FunctionEpilog(string Name) => new(Name);
    public static AsmLoadStack LoadStack(AsmStackOperand Src, AsmDestOperand Dst) => new(Src, Dst);
    public static AsmMov Mov(AsmOperand Src, AsmDestOperand Dst) => new(Src, Dst);
    public static AsmNeg Neg(AsmOperand Src) => new(Src);
    public static AsmProgram Program(string Filename) => new(Filename);
    public static AsmProgramEpilog ProgramEpilog() => new();
    public static AsmRet Ret() => new();
    public static AsmStoreStack StoreStack(AsmOperand Src, AsmStackOperand Dst) => new(Src, Dst);
    public static AsmImmOperand ImmOperand(int Imm) => new(Imm);
    public static AsmPseudoOperand PseudoOperand(string Name) => new(Name);
    public static AsmPseudoOperand PseudoOperand(TacVar v) => new(v);
    public static AsmRegOperand RegOperand(Register Reg) => new(Reg);
    public static AsmStackOperand StackOperand(int Offset) => new(Offset);
}
