using Wacc.Ast;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForPrefixOp(PrefixOp po)
    {
        var peOp = po.Op == TokenType.Increment ? TokenType.Plus : TokenType.Minus;
        var dst = ReserveTmpVar();
        var src1 = EmitTacky(po.LValExpr);
        Emit(new TacBinary(peOp, src1, new TacConstant(1), (TacVar)src1));
        Emit(new TacCopy(src1, dst));
        return dst;
    }
}


