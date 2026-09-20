using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForDefault(Default d)
    {
        EmitTacky(d.DefaultBlock);
        return DUMMY;
    }
}
