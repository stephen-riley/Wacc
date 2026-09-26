using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForIfElse(IfElse ie)
    {
        if (ie.ElseBlock is null)
        {
            var endLabel = ReserveTmpLabel();
            var cond = EmitTacky(ie.CondExpr);
            Emit(new TacJumpIfFalse(cond, endLabel));
            EmitTacky(ie.ThenBlock);
            EmitLabel(endLabel);
            return VOID;
        }
        else
        {
            var elseLabel = ReserveTmpLabel();
            var endLabel = ReserveTmpLabel();
            var cond = EmitTacky(ie.CondExpr);
            Emit(new TacJumpIfFalse(cond, elseLabel));
            EmitTacky(ie.ThenBlock);
            Emit(new TacJump(endLabel));
            EmitLabel(elseLabel);
            EmitTacky(ie.ElseBlock);
            EmitLabel(endLabel);
            return VOID;
        }
    }
}
