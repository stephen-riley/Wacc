using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForGoto(Goto g)
    {
        Emit(new TacJump(GetCleanLabelName(g.Label?.Name ?? throw new InvalidOperationException("BlockItem.LabelName cannot be null here"))));
        return VOID;
    }
}
