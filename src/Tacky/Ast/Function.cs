using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Function
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Function)node);

    private TacVal EmitTacky(TackyGenerator gen, Function f)
    {
        gen.instructions = [];
        gen.functions.Add(new TacFunction(f.Name, gen.instructions));
        gen.TmpVarCounter = 0;     // TODO: awkward here, shouldn't have to do this manually
        gen.EmitTacky(f.Body);
        gen.Emit(new TacReturn(new TacConstant(0)));
        return DUMMY;
    }
}