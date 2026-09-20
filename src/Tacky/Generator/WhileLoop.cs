using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForWhileLoop(WhileLoop wl)
    {
        var startLabel = ReserveTmpLabel("_wh");
        var endLabel = ReserveTmpLabel("_wh");
        BreakLabelStack.Push(endLabel);
        ContinueLabelStack.Push(startLabel);

        Emit(new TacLabel(startLabel));
        var condResult = EmitTacky(wl.CondExpr);
        Emit(new TacJumpIfZero(condResult, endLabel));
        EmitTacky(wl.BodyBlock);
        Emit(new TacJump(startLabel));
        Emit(new TacLabel(endLabel));

        BreakLabelStack.Pop();
        ContinueLabelStack.Pop();
        return DUMMY;
    }
}


