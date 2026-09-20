using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record BinaryOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (BinaryOp)node);

    private TacVal EmitTacky(TackyGenerator gen, BinaryOp b)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for BinaryOp is not implemented yet.");
    }
}

