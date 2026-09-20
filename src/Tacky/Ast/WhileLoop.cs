using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record WhileLoop
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (WhileLoop)node);

    private TacVal EmitTacky(TackyGenerator gen, WhileLoop w)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for WhileLoop is not implemented yet.");
    }
}

