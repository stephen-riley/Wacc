using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForLabeledBlock(LabeledBlock lb)
    {
        EmitLabel(GetCleanLabelName(lb.Label.Name));
        return VOID;
    }
}
