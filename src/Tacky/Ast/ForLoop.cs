using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record ForLoop
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (ForLoop)node);

    private TacVal EmitTacky(TackyGenerator gen, ForLoop fl)
    {
        var startLabel = gen.ReserveTmpLabel("_for");
        var contLabel = gen.ReserveTmpLabel("_for");
        var endLabel = gen.ReserveTmpLabel("_for");
        gen.BreakLabelStack.Push(endLabel);
        gen.ContinueLabelStack.Push(contLabel);

        endLabel = gen.ReserveTmpLabel("_for");
        gen.EmitTacky(fl.InitStat);
        gen.Emit(new TacLabel(startLabel));
        var cond = gen.EmitTacky(fl.CondExpr);
        gen.Emit(new TacJumpIfZero(cond, endLabel));
        gen.EmitTacky(fl.BodyBlock);
        gen.Emit(new TacLabel(contLabel));
        gen.EmitTacky(fl.PostStat);
        gen.Emit(new TacJump(startLabel));
        gen.Emit(new TacLabel(endLabel));

        gen.BreakLabelStack.Pop();
        gen.ContinueLabelStack.Pop();
        return DUMMY;
    }
}

