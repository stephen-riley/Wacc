using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Declaration
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Declaration)node);

    private TacVal EmitTacky(TackyGenerator gen, Declaration d)
    {
        if (d.Expr is null)
        {
            return gen.RegisterVar(new TacVar(d.Identifier.Name));
        }
        else
        {
            var declResult = gen.EmitTacky(d.Expr);
            gen.Emit(new TacCopy(declResult, gen.RegisterVar(new TacVar(d.Identifier.Name))));
            return declResult;
        }
    }
}

