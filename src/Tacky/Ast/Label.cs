using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Label
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Label)node);

    private TacVal EmitTacky(TackyGenerator gen, Label l) => gen.EmitWithDummy(new TacLabel(gen.GetCleanLabelName(l.Name)));
}

