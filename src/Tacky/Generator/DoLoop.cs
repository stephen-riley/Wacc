using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForDoLoop(DoLoop dl)
    {
        var startLabel = ReserveTmpLabel("_do");
        var condLabel = ReserveTmpLabel("_do");
        var endLabel = ReserveTmpLabel("_do");
        BreakLabelStack.Push(endLabel);
        ContinueLabelStack.Push(condLabel);

        Emit(new TacLabel(startLabel));
        EmitTacky(dl.BodyBlock);
        Emit(new TacLabel(condLabel));
        var condResult = EmitTacky(dl.CondExpr);
        Emit(new TacJumpIfNotZero(condResult, startLabel));
        Emit(new TacLabel(endLabel));

        BreakLabelStack.Pop();
        ContinueLabelStack.Pop();
        return DUMMY;
    }
}


