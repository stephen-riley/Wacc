using System.Text;
using Wacc.Ast;
using Wacc.CodeGen.AbstractAsm;

namespace Wacc.Emit;

public class CodeEmitter(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute()
    {
        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("EMIT ASM:");
        }

        if (Options.Verbose || Options.OnlyThroughCodeEmit)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;

            Emit(Options.AbstractAsm, stream);
        }

        return true;
    }

    internal static void Emit(IEnumerable<IAbstractAsm> asm, TextWriter stream)
    {
        foreach (var i in asm)
        {
            stream.WriteLine(Render(i));
        }
    }

    internal static string Render(IAbstractAsm asm) => asm switch
    {
        AsmBitNot abn => $"    mvn  {Render(abn.Src)}, {Render(abn.Src)}",
        AsmNeg an => $"    neg  {Render(an.Src)}, {Render(an.Src)}",
        AsmMov am => $"    mov  {Render(am.Dst)}, {Render(am.Src)}",
        AsmRet => "    ret",
        AsmImmOperand aio => $"#{aio.Imm}",
        AsmPseudoOperand apo => $"*{{{apo.Name}}}",
        _ => asm.EmitString()
    };
}