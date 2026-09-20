using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Block
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Block)node);

    private TacVal EmitTacky(TackyGenerator gen, Block b)
    {
        foreach (var bi in b.BlockItems)
        {
            gen.EmitTacky(bi);
        }
        return DUMMY;
    }
}

