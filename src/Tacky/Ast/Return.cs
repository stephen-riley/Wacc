using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Return
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Return)node);

    private TacVal EmitTacky(TackyGenerator gen, Return r)
    {
        var retResult = gen.EmitTacky(r.Expr);
        gen.Emit(new TacReturn(retResult));
        return retResult;
    }
}

