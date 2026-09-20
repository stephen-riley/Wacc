using Wacc.Ast;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForPostfixOp(PostfixOp po)
    {
        var poOp = po.Op == TokenType.Increment ? TokenType.Plus : TokenType.Minus;
        var dst = ReserveTmpVar();
        var src1 = EmitTacky(po.LValExpr);
        Emit(new TacCopy(src1, dst));
        Emit(new TacBinary(poOp, src1, new TacConstant(1), (TacVar)src1));
        return dst;
    }
}
