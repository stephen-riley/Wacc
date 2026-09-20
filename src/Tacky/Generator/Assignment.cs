using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForAssignment(Assignment a)
    {
        if (a.LExpr is Var v)
        {
            var rhsResult = EmitTacky(a.RExpr);
            Emit(new TacCopy(rhsResult, new TacVar(v.Name)));
            return new TacVar(v.Name);
        }
        else
        {
            throw new NotImplementedException("Tacky generation for complicated lvals is not implemented yet.");
        }
    }
}
