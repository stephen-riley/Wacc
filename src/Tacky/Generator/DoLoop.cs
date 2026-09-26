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

        EmitLabel(startLabel);
        EmitTacky(dl.BodyBlock);
        EmitLabel(condLabel);
        var condResult = EmitTacky(dl.CondExpr);
        Emit(new TacJumpIfTrue(condResult, startLabel));
        EmitLabel(endLabel);

        BreakLabelStack.Pop();
        ContinueLabelStack.Pop();
        return VOID;
    }
}
