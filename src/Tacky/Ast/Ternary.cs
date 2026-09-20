using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Ternary
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Ternary)node);

    private TacVal EmitTacky(TackyGenerator gen, Ternary t)
    {
        var altLabel = gen.ReserveTmpLabel();
        var endLabel = gen.ReserveTmpLabel();
        var result = gen.ReserveTmpVar();
        var cond = gen.EmitTacky(t.CondExpr);
        gen.Emit(new TacJumpIfZero(cond, altLabel));
        var middle = gen.EmitTacky(t.Middle);
        gen.Emit(new TacCopy(middle, result));
        gen.Emit(new TacJump(endLabel));
        gen.Emit(new TacLabel(altLabel));
        var right = gen.EmitTacky(t.Right);
        gen.Emit(new TacCopy(right, result));
        gen.Emit(new TacLabel(endLabel));
        return result;
    }
}

