using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record IfElse
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (IfElse)node);

    private TacVal EmitTacky(TackyGenerator gen, IfElse i)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for IfElse is not implemented yet.");
    }
}

