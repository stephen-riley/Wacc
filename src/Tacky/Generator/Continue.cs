using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForContinue(Continue c)
    {
        if (!BreakLabelStack.TryPeek(out var contLabel))
        {
            throw new TackyGenError("no break label in this scope");
        }
        Emit(new TacJump(contLabel));
        return VOID;
    }
}
