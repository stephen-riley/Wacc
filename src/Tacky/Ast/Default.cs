using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Default
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Default)node);

    private TacVal EmitTacky(TackyGenerator gen, Default d)
    {
        gen.EmitTacky(d.DefaultBlock);
        return DUMMY;
    }
}

