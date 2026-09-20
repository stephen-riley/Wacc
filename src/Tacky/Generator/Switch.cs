using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForSwitch(Switch sw)
    {
        var endLabel = ReserveTmpLabel("_sc");
        BreakLabelStack.Push(endLabel);

        var condVar = EmitTacky(sw.CondExpr);
        var block = sw.CaseBlock as Block ?? throw new TackyGenError($"body of switch-case is not a Block");

        var nextLabel = ReserveTmpLabel("_sc");
        foreach (var stat in block.BlockItems)
        {
            if (stat is Case c)
            {
                var caseVal = EmitTacky(c.CaseCondExpr);
                var dest = ReserveTmpVar();
                Emit(new TacBinary(TokenType.EqualTo, condVar, caseVal, dest));
                Emit(new TacJumpIfNotZero(dest, nextLabel));
                EmitTacky(c.CaseBlock);
                Emit(new TacLabel(nextLabel));
                nextLabel = ReserveTmpLabel("_sc");
            }
            else if (stat is Default d)
            {
                EmitTacky(d.DefaultBlock);
            }
        }
        Emit(new TacLabel(endLabel));

        BreakLabelStack.Pop();
        return DUMMY;
    }
}


