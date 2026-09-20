using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record PrefixOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (PrefixOp)node);

    private TacVal EmitTacky(TackyGenerator gen, PrefixOp p)
    {
        var peOp = p.Op == TokenType.Increment ? TokenType.Plus : TokenType.Minus;
        var dst = gen.ReserveTmpVar();
        var src1 = gen.EmitTacky(p.LValExpr);
        gen.Emit(new TacBinary(peOp, src1, new TacConstant(1), (TacVar)src1));
        gen.Emit(new TacCopy(src1, dst));
        return dst;
    }
}

