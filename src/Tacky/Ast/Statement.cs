using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Statement
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Statement)node);

    private TacVal EmitTacky(TackyGenerator gen, Statement s)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Statement is not implemented yet.");
    }
}

