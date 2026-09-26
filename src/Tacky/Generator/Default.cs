using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForDefault(Default d)
    {
        if (!SwitchContextStack.TryPeek(out var swc))
        {
            throw new TackyGenError("Default case outside of switch statement");
        }

        EmitLabel(swc.NextCaseEvalLabel);
        // EmitTacky(d.DefaultBlock);

        return VOID;
    }
}
