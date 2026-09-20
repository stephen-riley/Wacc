using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record PostfixOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (PostfixOp)node);

    private TacVal EmitTacky(TackyGenerator gen, PostfixOp po)
    {
        var poOp = po.Op == TokenType.Increment ? TokenType.Plus : TokenType.Minus;
        var dst = gen.ReserveTmpVar();
        var src1 = gen.EmitTacky(po.LValExpr);
        gen.Emit(new TacCopy(src1, dst));
        gen.Emit(new TacBinary(poOp, src1, new TacConstant(1), (TacVar)src1));
        return dst;
    }
}

