using Wacc.Exceptions;
using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Continue
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Continue)node);

    private TacVal EmitTacky(TackyGenerator gen, Continue c)
    {
        if (!gen.BreakLabelStack.TryPeek(out var contLabel))
        {
            throw new TackyGenError("no break label in this scope");
        }
        gen.Emit(new TacJump(contLabel));
        return DUMMY;
    }
}

