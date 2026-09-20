using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Var
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Var)node);

    private TacVal EmitTacky(TackyGenerator gen, Var v) => gen.RegisterVar(new TacVar(v.Name));
}

