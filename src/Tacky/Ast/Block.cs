using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Block
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Block)node);

    private TacVal EmitTacky(TackyGenerator gen, Block b)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Block is not implemented yet.");
    }
}

