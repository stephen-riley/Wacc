using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForBlock(Block b)
    {
        foreach (var bi in b.BlockItems)
        {
            EmitTacky(bi);
        }
        return DUMMY;
    }
}


