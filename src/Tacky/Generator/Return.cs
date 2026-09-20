using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForReturn(Return r)
    {
        var retResult = EmitTacky(r.Expr);
        Emit(new TacReturn(retResult));
        return retResult;
    }
}
