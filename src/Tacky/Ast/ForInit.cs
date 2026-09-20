using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record ForInit
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (ForInit)node);

    private TacVal EmitTacky(TackyGenerator gen, ForInit f)
    {
        throw new NotImplementedException("Tacky generation for ForInit is not implemented yet.");
    }
}

