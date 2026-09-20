using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record IfElse
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (IfElse)node);

    private TacVal EmitTacky(TackyGenerator gen, IfElse ie)
    {
        if (ie.ElseBlock is null)
        {
            var endLabel = gen.ReserveTmpLabel();
            var cond = gen.EmitTacky(ie.CondExpr);
            gen.Emit(new TacJumpIfZero(cond, endLabel));
            gen.EmitTacky(ie.ThenBlock);
            gen.Emit(new TacLabel(endLabel));
            return DUMMY;
        }
        else
        {
            var elseLabel = gen.ReserveTmpLabel();
            var endLabel = gen.ReserveTmpLabel();
            var cond = gen.EmitTacky(ie.CondExpr);
            gen.Emit(new TacJumpIfZero(cond, elseLabel));
            gen.EmitTacky(ie.ThenBlock);
            gen.Emit(new TacJump(endLabel));
            gen.Emit(new TacLabel(elseLabel));
            gen.EmitTacky(ie.ElseBlock);
            gen.Emit(new TacLabel(endLabel));
            return DUMMY;
        }
    }
}

