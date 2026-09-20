using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record DoLoop
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (DoLoop)node);

    private TacVal EmitTacky(TackyGenerator gen, DoLoop d)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for DoLoop is not implemented yet.");
    }
}

