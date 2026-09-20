using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Constant
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Constant)node);

    private TacVal EmitTacky(TackyGenerator _, Constant c) => new TacConstant(c.Int);
}

