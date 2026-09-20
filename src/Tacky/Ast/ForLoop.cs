using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record ForLoop
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (ForLoop)node);

    private TacVal EmitTacky(TackyGenerator gen, ForLoop f)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for ForLoop is not implemented yet.");
    }
}

