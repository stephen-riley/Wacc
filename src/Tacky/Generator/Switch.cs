using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;

namespace Wacc.Tacky;

public class SwitchContext
{
    public Switch SwitchNode = null!;
    public TacVal CondEvalVar = null!;
    public bool StartedCases = false;
    public string NextCaseEvalLabel = null!;
    public string NextCaseBlockLabel = null!;
    public string EndLabel = null!;
}

public partial class TackyGenerator
{
    private TacVal EmityTackyForSwitch(Switch sw)
    {
        EmitLabel(ReserveTmpLabel("_sw"));

        var swc = new SwitchContext()
        {
            SwitchNode = sw,
            NextCaseEvalLabel = ReserveTmpLabel("_sce"),
            NextCaseBlockLabel = ReserveTmpLabel("_scb"),
            EndLabel = ReserveTmpLabel("_sw"),
        };

        SwitchContextStack.Push(swc);
        BreakLabelStack.Push(swc.EndLabel);

        swc.CondEvalVar = EmitTacky(sw.CondExpr);
        EmitTacky(sw.SwitchBlock);

        EmitLabel(swc.NextCaseEvalLabel);
        EmitLabel(swc.EndLabel);

        BreakLabelStack.Pop();
        SwitchContextStack.Pop();

        return VOID;
    }
}
