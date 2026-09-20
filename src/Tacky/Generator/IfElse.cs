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
            Emit(new TacJumpIfZero(cond, endLabel));
            EmitTacky(ie.ThenBlock);
            Emit(new TacLabel(endLabel));
            return DUMMY;
        }
        else
        {
            var elseLabel = ReserveTmpLabel();
            var endLabel = ReserveTmpLabel();
            var cond = EmitTacky(ie.CondExpr);
            Emit(new TacJumpIfZero(cond, elseLabel));
            EmitTacky(ie.ThenBlock);
            Emit(new TacJump(endLabel));
            Emit(new TacLabel(elseLabel));
            EmitTacky(ie.ElseBlock);
            Emit(new TacLabel(endLabel));
            return DUMMY;
        }
    }
}


