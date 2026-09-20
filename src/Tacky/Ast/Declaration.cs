using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Declaration
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Declaration)node);

    private TacVal EmitTacky(TackyGenerator gen, Declaration d)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Declaration is not implemented yet.");
    }
}

