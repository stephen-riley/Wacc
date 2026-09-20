using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record CompUnit
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (CompUnit)node);

    private TacVal EmitTacky(TackyGenerator gen, CompUnit p)
    {
        foreach (var s in p.Functions)
        {
            gen.EmitTacky(s);
        }
        return DUMMY;
    }
}

