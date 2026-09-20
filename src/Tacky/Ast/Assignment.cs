using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Assignment
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Assignment)node);

    private TacVal EmitTacky(TackyGenerator gen, Assignment a)
    {
        if (a.LExpr is Var v)
        {
            var rhsResult = gen.EmitTacky(a.RExpr);
            gen.Emit(new TacCopy(rhsResult, new TacVar(v.Name)));
            return new TacVar(v.Name);
        }
        else
        {
            throw new NotImplementedException("Tacky generation for complicated lvals is not implemented yet.");
        }
    }
}

