using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record PostfixOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (PostfixOp)node);

    private TacVal EmitTacky(TackyGenerator gen, PostfixOp p)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for PostfixOp is not implemented yet.");
    }
}

