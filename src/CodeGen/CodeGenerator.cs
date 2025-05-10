using Wacc.AbstractAsm;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options => opts;

    internal List<IAbstractAsm> Asm = [];

    public bool Execute()
    {
        var asm = new List<IAbstractAsm>();

        asm.AddRange(Translate(Options.Tacky));

        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("ABSTRACT ASM:");
        }

        if (Options.Verbose || Options.OnlyThroughCodeGen)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;

            foreach (var a in asm)
            {
                stream.WriteLine(a);
            }
        }

        return true;
    }

    internal IEnumerable<IAbstractAsm> Translate(ITackyInstr instr)
    {
        var bundle = new List<IAbstractAsm>();

        switch (instr)
        {
            case TacProgram p:
                return [new ProgramGen()];

            case TacFunction f:
                return [new FunctionGen(f.Name, f.Instructions)];

            default:
                throw new CodeGenError($"{nameof(CodeGenerator)}.{nameof(Translate)} can't handle type {instr.GetType().Name} yet");

        }
    }
}