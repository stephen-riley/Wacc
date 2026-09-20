using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record OpAssignment
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (OpAssignment)node);

    private TacVal EmitTacky(TackyGenerator gen, OpAssignment o)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for OpAssignment is not implemented yet.");
    }
}

