using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForCompUnit(CompUnit cu)
    {
        foreach (var s in cu.Functions)
        {
            EmitTacky(s);
        }
        return DUMMY;
    }
}


