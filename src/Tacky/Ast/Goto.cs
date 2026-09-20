using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Goto
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Goto)node);

    private TacVal EmitTacky(TackyGenerator gen, Goto g)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Goto is not implemented yet.");
    }
}

