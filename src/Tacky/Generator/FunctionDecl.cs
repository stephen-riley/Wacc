using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmitTackyForFunctionDecl(FunctionDecl f)
    {
        instructions = [];
        functions.Add(new TacFunction(f.Identifier.Name, instructions));
        TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually
        if (f.Body is not null)
        {
            EmitTacky(f.Body);
        }
        Emit(new TacReturn(new TacConstant(0)));
        return VOID;
    }
}
