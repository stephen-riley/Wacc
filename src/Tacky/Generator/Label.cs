using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForLabel(Label l) => EmitWithVoid(new TacLabel(GetCleanLabelName(l.Name)));
}
