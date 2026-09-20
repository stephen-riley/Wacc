using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Return
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Return)node);

    private TacVal EmitTacky(TackyGenerator gen, Return r)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Return is not implemented yet.");
    }
}

