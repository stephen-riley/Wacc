using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record BlockItem
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (BlockItem)node);

    private TacVal EmitTacky(TackyGenerator gen, BlockItem b)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for BlockItem is not implemented yet.");
    }
}

