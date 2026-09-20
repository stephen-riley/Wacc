using Wacc.Exceptions;
using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record Switch
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (Switch)node);

    private TacVal EmitTacky(TackyGenerator gen, Switch sw)
    {
        var endLabel = gen.ReserveTmpLabel("_sc");
        gen.BreakLabelStack.Push(endLabel);

        var condVar = gen.EmitTacky(sw.CondExpr);
        var block = sw.CaseBlock as Block ?? throw new TackyGenError($"body of switch-case is not a Block");

        var nextLabel = gen.ReserveTmpLabel("_sc");
        foreach (var stat in block.BlockItems)
        {
            if (stat is Case c)
            {
                var caseVal = gen.EmitTacky(c.CaseCondExpr);
                var dest = gen.ReserveTmpVar();
                gen.Emit(new TacBinary(TokenType.EqualTo, condVar, caseVal, dest));
                gen.Emit(new TacJumpIfNotZero(dest, nextLabel));
                gen.EmitTacky(c.CaseBlock);
                gen.Emit(new TacLabel(nextLabel));
                nextLabel = gen.ReserveTmpLabel("_sc");
            }
            else if (stat is Default d)
            {
                gen.EmitTacky(d.DefaultBlock);
            }
        }
        gen.Emit(new TacLabel(endLabel));

        gen.BreakLabelStack.Pop();
        return DUMMY;
    }
}

