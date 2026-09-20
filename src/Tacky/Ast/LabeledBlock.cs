using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record LabeledBlock
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (LabeledBlock)node);

    private TacVal EmitTacky(TackyGenerator gen, LabeledBlock l)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for LabeledBlock is not implemented yet.");
    }
}

