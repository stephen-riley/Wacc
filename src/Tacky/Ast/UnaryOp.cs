using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record UnaryOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (UnaryOp)node);

    private TacVal EmitTacky(TackyGenerator gen, UnaryOp u)
    {
        var src = gen.EmitTacky(u.Expr);
        var dst = gen.ReserveTmpVar();
        gen.Emit(new TacUnary(u.Op.TokenType, src, dst));
        return dst;
    }
}

