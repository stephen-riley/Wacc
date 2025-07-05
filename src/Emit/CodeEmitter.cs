using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;

namespace Wacc.Emit;

public class CodeEmitter(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute()
    {
        using var sw = new StringWriter();
        Emit(Options.AbstractAsm, sw);
        Options.Assembly = sw.ToString();

        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("EMIT ASM:");
            Console.Error.WriteLine("==========");
        }

        if (Options.Verbose || Options.OnlyThroughCodeEmit || Options.Assemble)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;
            stream.Write(Options.Assembly);
        }

        return true;
    }

    internal static void Emit(IEnumerable<AsmObject> asm, TextWriter stream)
    {
        foreach (var i in asm)
        {
            if (i is not AsmComment)
            {
                i.EmitArm(stream);
            }
        }
    }
}