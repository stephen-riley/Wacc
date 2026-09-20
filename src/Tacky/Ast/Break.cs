using Wacc.Exceptions;
using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Break
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Break)node);

    private TacVal EmitTacky(TackyGenerator gen, Break b)
    {
        if (!gen.BreakLabelStack.TryPeek(out var breakLabel))
        {
            throw new TackyGenError("no break label in this scope");
        }
        gen.Emit(new TacJump(breakLabel));
        return DUMMY;
    }
}

