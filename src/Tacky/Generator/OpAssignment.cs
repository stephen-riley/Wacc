using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForOpAssignment(OpAssignment oa)
    {
        if (oa.LExpr is Var v)
        {
            var rhsResult = EmitTacky(oa.RExpr);
            return new TacVar(v.Name);
        }
        else
        {
            throw new NotImplementedException("Tacky generation for complicated lvals is not implemented yet.");
        }
    }
}


