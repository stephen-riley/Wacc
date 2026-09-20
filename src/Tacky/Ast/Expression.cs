using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record Expression
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Expression)node);

    private TacVal EmitTacky(TackyGenerator gen, Expression e)
    {
        gen.instructions = [];
        throw new NotImplementedException("Tacky generation for Expression is not implemented yet.");
    }
}

