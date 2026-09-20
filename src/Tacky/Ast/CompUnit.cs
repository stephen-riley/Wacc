using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record CompUnit
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (CompUnit)node);

    private TacVal EmitTacky(TackyGenerator gen, CompUnit c)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for CompUnit is not implemented yet.");
    }
}

