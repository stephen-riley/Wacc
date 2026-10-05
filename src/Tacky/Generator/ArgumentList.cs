using Wacc.Ast;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmitTackyForArgumentList(ArgumentList al)
    {
        foreach (var p in al.Params)
        {
            foreach (var c in p.Children())
            {
                EmitTacky(c);
            }
        }

        return VOID;
    }
}
