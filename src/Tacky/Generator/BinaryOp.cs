using Wacc.Ast;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForBinaryOp(BinaryOp b)
    {
        if (b.Op == TokenType.LogicalAnd)
        {
            return EmitLogicalAnd(b);
        }
        else if (b.Op == TokenType.LogicalOr)
        {
            return EmitLogicalOr(b);
        }
        else
        {
            var src1 = EmitTacky(b.LExpr);
            var src2 = EmitTacky(b.RExpr);
            var dst = ReserveTmpVar();
            Emit(new TacBinary(b.Op, src1, src2, dst));
            return dst;
        }
    }

    internal TacVar EmitLogicalAnd(BinaryOp b)
    {
        var falseLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var v1 = ReserveTmpVar();
        var result = ReserveTmpVar();

        // evaluate lexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var lexpr = EmitTacky(b.LExpr);
        Emit(new TacCopy(lexpr, v1));
        Emit(new TacJumpIfZero(v1, falseLabel));

        // evaluate rexpr.  If false, jump to `falseLabel`; otherwise, fall through.
        var rexpr = EmitTacky(b.RExpr);
        Emit(new TacCopy(rexpr, v1));
        Emit(new TacJumpIfZero(v1, falseLabel));

        // set result to 1 and jump to `endLabel`
        Emit(new TacCopy(new TacConstant(1), result));
        Emit(new TacJump(endLabel));

        // set result to 0 and fall through to end
        Emit(new TacLabel(falseLabel));
        Emit(new TacCopy(new TacConstant(0), result));
        Emit(new TacLabel(endLabel));

        return result;
    }

    internal TacVar EmitLogicalOr(BinaryOp b)
    {
        var trueLabel = ReserveTmpLabel();
        var endLabel = ReserveTmpLabel();
        var v1 = ReserveTmpVar();
        var result = ReserveTmpVar();

        // Evaluate lexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        var lexpr = EmitTacky(b.LExpr);
        Emit(new TacCopy(lexpr, v1));
        Emit(new TacJumpIfNotZero(v1, trueLabel));

        // Evaluate rexpr.  If true, jump to `trueLabel`; otherwise, fall through.
        var rexpr = EmitTacky(b.RExpr);
        Emit(new TacCopy(rexpr, v1));
        Emit(new TacJumpIfNotZero(v1, trueLabel));

        // set result to 0 and jump to `endLabel`
        Emit(new TacCopy(new TacConstant(0), result));
        Emit(new TacJump(endLabel));

        // set result to 1 and fall through to end
        Emit(new TacLabel(trueLabel));
        Emit(new TacCopy(new TacConstant(1), result));
        Emit(new TacLabel(endLabel));

        return result;
    }
}
