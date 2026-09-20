using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record LabeledBlock
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (LabeledBlock)node);

    private TacVal EmitTacky(TackyGenerator gen, LabeledBlock l)
    {
        gen.Emit(new TacLabel(gen.GetCleanLabelName(l.Label.Name)));
        return DUMMY;
    }
}