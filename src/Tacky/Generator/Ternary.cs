using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForTernary(Ternary t)
    {
        var altLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var result = ReserveTmpVar();
        var cond = EmitTacky(t.CondExpr);
        Emit(new TacJumpIfZero(cond, altLabel));
        var middle = EmitTacky(t.Middle);
        Emit(new TacCopy(middle, result));
        Emit(new TacJump(endLabel));
        EmitLabel(altLabel);
        var right = EmitTacky(t.Right);
        Emit(new TacCopy(right, result));
        EmitLabel(endLabel);
        return result;
    }
}
