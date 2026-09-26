using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForForLoop(ForLoop fl)
    {
        var startLabel = ReserveTmpLabel("_for");
        var contLabel = ReserveTmpLabel("_for");
        var endLabel = ReserveTmpLabel("_for");
        BreakLabelStack.Push(endLabel);
        ContinueLabelStack.Push(contLabel);

        endLabel = ReserveTmpLabel("_for");
        EmitTacky(fl.InitStat);
        EmitLabel(startLabel);
        if (fl.CondExpr is not NullStatement)
        {
            var cond = EmitTacky(fl.CondExpr);
            Emit(new TacJumpIfZero(cond, endLabel));
        }
        EmitTacky(fl.BodyBlock);
        EmitLabel(contLabel);
        EmitTacky(fl.PostStat);
        Emit(new TacJump(startLabel));
        EmitLabel(endLabel);

        BreakLabelStack.Pop();
        ContinueLabelStack.Pop();
        return VOID;
    }
}
