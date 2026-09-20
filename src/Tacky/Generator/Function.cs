using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyForFunction(Function f)
    {
        instructions = [];
        functions.Add(new TacFunction(f.Name, instructions));
        TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually
        EmitTacky(f.Body);
        Emit(new TacReturn(new TacConstant(0)));
        return DUMMY;
    }
}
