using Wacc.Tacky;
using Wacc.Tacky.Instruction;

namespace Wacc.Ast;

public partial record AstNode
{
    public abstract TacVal EmitTacky(TackyGenerator gen, AstNode node);
}