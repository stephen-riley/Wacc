using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmitTackyForFunctionCall(FunctionCall fc)
    {
        var dst = ReserveTmpVar();

        EmitTacky(fc.Args);
        Emit(new TacFuncCall(fc.Identifier.Name, dst));
        return dst;
    }
}
