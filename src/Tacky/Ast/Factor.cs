using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Factor
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Factor)node);

    private TacVal EmitTacky(TackyGenerator gen, Factor f)
    {
        throw new NotImplementedException("Tacky generation for Factor is not implemented yet.");
    }
}

