using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmitTackyForFunctionDecl(FunctionDecl f)
    {
        if (f.Body is not null)
        {
            instructions = [];
            functions.Add(new TacFunction(f.Identifier.Name, instructions));
            TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually

            EmitLabel($"_{f.Identifier.Name}");
            EmitTacky(f.Body);

            // This TacReturn will get optimized out in later stages if all paths have an explicit return
            Emit(new TacReturn(new TacConstant(0)));
        }
        return VOID;
    }
}
