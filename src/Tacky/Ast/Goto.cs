using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Goto
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Goto)node);

    private TacVal EmitTacky(TackyGenerator gen, Goto g)
    {
        gen.Emit(new TacJump(gen.GetCleanLabelName(g.Label?.Name ?? throw new InvalidOperationException("BlockItem.LabelName cannot be null here"))));
        return DUMMY;
    }
}

