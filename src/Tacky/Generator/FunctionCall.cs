using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmitTackyForFunctionCall(FunctionCall fc)
    {
        EmitTacky(fc.Args);

        throw new NotImplementedException("Tacky generation for function calls is not implemented yet.");
    }
}
