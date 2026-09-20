using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record OpAssignment
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (OpAssignment)node);

    private TacVal EmitTacky(TackyGenerator gen, OpAssignment oa)
    {
        if (oa.LExpr is Var v)
        {
            var rhsResult = gen.EmitTacky(oa.RExpr);
            return new TacVar(v.Name);
        }
        else
        {
            throw new NotImplementedException("Tacky generation for complicated lvals is not implemented yet.");
        }
    }
}

