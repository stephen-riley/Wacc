using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record WhileLoop
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (WhileLoop)node);

    private TacVal EmitTacky(TackyGenerator gen, WhileLoop wl)
    {
        var startLabel = gen.ReserveTmpLabel("_wh");
        var endLabel = gen.ReserveTmpLabel("_wh");
        gen.BreakLabelStack.Push(endLabel);
        gen.ContinueLabelStack.Push(startLabel);

        gen.Emit(new TacLabel(startLabel));
        var condResult = gen.EmitTacky(wl.CondExpr);
        gen.Emit(new TacJumpIfZero(condResult, endLabel));
        gen.EmitTacky(wl.BodyBlock);
        gen.Emit(new TacJump(startLabel));
        gen.Emit(new TacLabel(endLabel));

        gen.BreakLabelStack.Pop();
        gen.ContinueLabelStack.Pop();
        return DUMMY;
    }
}

