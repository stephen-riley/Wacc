using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record NullStatement
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (NullStatement)node);

    private TacVal EmitTacky(TackyGenerator gen, NullStatement n)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for NullStatement is not implemented yet.");
    }
}

