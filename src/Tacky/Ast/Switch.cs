using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Switch
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Switch)node);

    private TacVal EmitTacky(TackyGenerator gen, Switch s)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Switch is not implemented yet.");
    }
}

