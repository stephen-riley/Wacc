using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Constant
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Constant)node);

    private TacVal EmitTacky(TackyGenerator gen, Constant c)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Constant is not implemented yet.");
    }
}

