using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForDeclaration(Declaration d)
    {
        if (d.Expr is null)
        {
            return RegisterVar(new TacVar(d.Identifier.Name));
        }
        else
        {
            var declResult = EmitTacky(d.Expr);
            Emit(new TacCopy(declResult, RegisterVar(new TacVar(d.Identifier.Name))));
            return declResult;
        }
    }
}
