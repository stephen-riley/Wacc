using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Case
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Case)node);

    private TacVal EmitTacky(TackyGenerator gen, Case c)
    {
        throw new NotImplementedException("Tacky generation for Case is not implemented yet.");
    }
}

