using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Continue
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Continue)node);

    private TacVal EmitTacky(TackyGenerator gen, Continue c)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Continue is not implemented yet.");
    }
}

