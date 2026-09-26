using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForBreak(Break b)
    {
        if (!BreakLabelStack.TryPeek(out var breakLabel))
        {
            throw new TackyGenError("no break label in this scope");
        }

        Emit(new TacJump(breakLabel));

        return VOID;
    }
}
