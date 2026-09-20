using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record DoLoop
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (DoLoop)node);

    private TacVal EmitTacky(TackyGenerator gen, DoLoop dl)
    {
        var startLabel = gen.ReserveTmpLabel("_do");
        var condLabel = gen.ReserveTmpLabel("_do");
        var endLabel = gen.ReserveTmpLabel("_do");
        gen.BreakLabelStack.Push(endLabel);
        gen.ContinueLabelStack.Push(condLabel);

        gen.Emit(new TacLabel(startLabel));
        gen.EmitTacky(dl.BodyBlock);
        gen.Emit(new TacLabel(condLabel));
        var condResult = gen.EmitTacky(dl.CondExpr);
        gen.Emit(new TacJumpIfNotZero(condResult, startLabel));
        gen.Emit(new TacLabel(endLabel));

        gen.BreakLabelStack.Pop();
        gen.ContinueLabelStack.Pop();
        return DUMMY;
    }
}

