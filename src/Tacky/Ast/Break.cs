using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Break
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Break)node);

    private TacVal EmitTacky(TackyGenerator gen, Break b)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Break is not implemented yet.");
    }
}

