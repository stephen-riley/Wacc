using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record UnaryOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (UnaryOp)node);

    private TacVal EmitTacky(TackyGenerator gen, UnaryOp u)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for UnaryOp is not implemented yet.");
    }
}

