using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForUnaryOp(UnaryOp uo)
    {
        var src = EmitTacky(uo.Expr);
        var dst = ReserveTmpVar();
        Emit(new TacUnary(uo.Op.TokenType, src, dst));
        return dst;
    }
}
