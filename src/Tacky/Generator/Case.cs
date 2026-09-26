using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForCase(Case c)
    {
        if (!SwitchContextStack.TryPeek(out var swc))
        {
            throw new TackyGenError("Case statement is not inside a switch statement");
        }

        if (swc.StartedCases)
        {
            Emit(new TacJump(swc.NextCaseBlockLabel));
        }
        else
        {
            swc.StartedCases = true;
        }

        var dst = ReserveTmpVar();

        EmitLabelAndGetNext(ref swc.NextCaseEvalLabel, "_sce");
        var caseCondVar = EmitTacky(c.CaseCondExpr);

        Emit(new TacBinary(TokenType.EqualTo, caseCondVar, swc.CondEvalVar, dst));
        Emit(new TacJumpIfFalse(dst, swc.NextCaseEvalLabel));

        EmitLabelAndGetNext(ref swc.NextCaseBlockLabel, "_scb");
        // EmitTacky(c.CaseBlock);

        return VOID;
    }
}
