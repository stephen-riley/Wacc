using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Ternary
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Ternary)node);

    private TacVal EmitTacky(TackyGenerator gen, Ternary t)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Ternary is not implemented yet.");
    }
}

