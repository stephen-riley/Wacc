using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record BinaryOp
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (BinaryOp)node);

    private TacVal EmitTacky(TackyGenerator gen, BinaryOp b)
    {
        if (b.Op == TokenType.LogicalAnd)
        {
            return EmitLogicalAnd(gen, b);
        }
        else if (b.Op == TokenType.LogicalOr)
        {
            return EmitLogicalOr(gen, b);
        }
        else
        {
            var src1 = gen.EmitTacky(b.LExpr);
            var src2 = gen.EmitTacky(b.RExpr);
            var dst = gen.ReserveTmpVar();
            gen.Emit(new TacBinary(b.Op, src1, src2, dst));
            return dst;
        }
    }

    internal TacVar EmitLogicalAnd(TackyGenerator gen, BinaryOp b)
    {
        var falseLabel = gen.ReserveTmpLabel();
        var endLabel = gen.ReserveTmpLabel();
        var v1 = gen.ReserveTmpVar();
        var result = gen.ReserveTmpVar();

        // evaluate lexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var lexpr = gen.EmitTacky(b.LExpr);
        gen.Emit(new TacCopy(lexpr, v1));
        gen.Emit(new TacJumpIfZero(v1, falseLabel));

        // evaluate rexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var rexpr = gen.EmitTacky(b.RExpr);
        gen.Emit(new TacCopy(rexpr, v1));
        gen.Emit(new TacJumpIfZero(v1, falseLabel));

        // set result to 1 and jump to `endLabel`
        gen.Emit(new TacCopy(new TacConstant(1), result));
        gen.Emit(new TacJump(endLabel));

        // set result to 0 and fall through to end
        gen.Emit(new TacLabel(falseLabel));
        gen.Emit(new TacCopy(new TacConstant(0), result));
        gen.Emit(new TacLabel(endLabel));

        return result;
    }

    internal TacVar EmitLogicalOr(TackyGenerator gen, BinaryOp b)
    {
        var trueLabel = gen.ReserveTmpLabel();
        var endLabel = gen.ReserveTmpLabel();
        var v1 = gen.ReserveTmpVar();
        var result = gen.ReserveTmpVar();

        // Evaluate lexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        var lexpr = gen.EmitTacky(b.LExpr);
        gen.Emit(new TacCopy(lexpr, v1));
        gen.Emit(new TacJumpIfNotZero(v1, trueLabel));

        // Evaluate rexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        var rexpr = gen.EmitTacky(b.RExpr);
        gen.Emit(new TacCopy(rexpr, v1));
        gen.Emit(new TacJumpIfNotZero(v1, trueLabel));

        // set result to 0 and jump to `endLabel`
        gen.Emit(new TacCopy(new TacConstant(0), result));
        gen.Emit(new TacJump(endLabel));

        // set result to 1 and fall through to end
        gen.Emit(new TacLabel(trueLabel));
        gen.Emit(new TacCopy(new TacConstant(1), result));
        gen.Emit(new TacLabel(endLabel));

        return result;
    }
}

