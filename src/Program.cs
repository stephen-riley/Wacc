using CommandLine;
using Wacc;
using Wacc.Emit;
using Wacc.Exe;
using Wacc.Lex;

Console.Error.WriteLine("Wacc 1.0");

Parser.Default.ParseArguments<RuntimeState>(args)
    .WithParsed(rts =>
    {
        rts.Text = File.ReadAllText(rts.InputFile);
        Entrypoint(rts);
    });
// .WithNotParsed(HandleParseError);

static void Entrypoint(RuntimeState rts)
{
    Console.Error.WriteLine("CLI: " + Parser.Default.FormatCommandLine(rts));

    try
    {
        if (rts.DoLexer || rts.DoAll)
        {
            if (rts.Verbose) Console.Error.WriteLine("\nStage: Lex");
            new Lexer(rts).Execute();
        }

        if (rts.DoParser || rts.DoAll)
        {
            if (rts.Verbose) Console.Error.WriteLine("\nStage: Parse");
            new Wacc.Parse.Parser(rts).Execute();
        }

        if (rts.DoTacky || rts.DoAll)
        {
            if (rts.Verbose) Console.Error.WriteLine("\nStage: TAC IR gen");
            new Wacc.Tacky.TackyGenerator(rts).Execute();
        }

        if (rts.DoCodeGen || rts.DoAll)
        {
            if (rts.Verbose) Console.Error.WriteLine("\nStage: Asm IR gen");
            new Wacc.CodeGen.CodeGenerator(rts).Execute();
        }

        if (rts.DoCodeEmission || rts.Assemble || rts.DoAll)
        {
            if (rts.Verbose) Console.Error.WriteLine("\nStage: Asm emit");
            new CodeEmitter(rts).Execute();
        }

        if (rts.Assemble)
        {

        }

        if (rts.DoAll)
        {
            if (rts.Verbose) Console.Error.WriteLine("\nStage: assemble");
            new GenExecutable(rts).Execute();
        }
    }
    catch (Exception e)
    {
        var eName = e.GetType().Name;
        Console.Error.WriteLine($"\n{eName}: {e.Message}");
        if (!eName.EndsWith("Error"))
        {
            Console.Error.WriteLine(e.StackTrace);
        }
        Environment.Exit(1);
    }
}