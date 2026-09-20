using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record NullStatement
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (NullStatement)node);

    private TacVal EmitTacky(TackyGenerator gen, NullStatement n) => DUMMY;
}

