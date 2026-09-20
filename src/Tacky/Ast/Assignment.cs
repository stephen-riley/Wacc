using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Assignment
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Assignment)node);

    private TacVal EmitTacky(TackyGenerator gen, Assignment a)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Assignment is not implemented yet.");
    }
}

