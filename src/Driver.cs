using CommandLine;
using Wacc.CodeGen;
using Wacc.Emit;
using Wacc.Exe;
using Wacc.Lex;
using Wacc.Tacky;

namespace Wacc;

public class Driver(RuntimeState options)
{
    internal RuntimeState Rts => options;

    public void Entrypoint()
    {
        if (Rts.Verbose)
        {
            Console.Error.WriteLine("Wacc 1.0");
            Console.Error.WriteLine("CLI: " + Parser.Default.FormatCommandLine(Rts));
        }

        if (Rts.DoLexer || Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: Lex");
            new Lexer(Rts).Execute();
        }

        if (Rts.DoParser || Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: Parse");
            new Parse.Parser(Rts).Execute();
        }

        if (Rts.DoValidate || Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: Validation");
            new SemanticAnalyzer(Rts).Execute();
        }

        if (Rts.DoTacky || Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: TAC IR gen");
            new TackyGenerator(Rts).Execute();
        }

        if (Rts.DoCodeGen || Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: Asm IR gen");
            new CodeGenerator(Rts).Execute();
        }

        if (Rts.DoCodeEmission || Rts.Assemble || Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: Asm emit");
            new CodeEmitter(Rts).Execute();
        }

        if (Rts.DoAll)
        {
            if (Rts.Verbose) Console.Error.WriteLine("\nStage: assemble");
            new GenExecutable(Rts).Execute();
        }
    }
}