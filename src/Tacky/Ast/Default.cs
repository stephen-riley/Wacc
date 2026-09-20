using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Default
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Default)node);

    private TacVal EmitTacky(TackyGenerator gen, Default d)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Default is not implemented yet.");
    }
}

