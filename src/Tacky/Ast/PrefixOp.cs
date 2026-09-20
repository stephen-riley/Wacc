using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record PrefixOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (PrefixOp)node);

    private TacVal EmitTacky(TackyGenerator gen, PrefixOp p)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for PrefixOp is not implemented yet.");
    }
}

