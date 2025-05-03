using CommandLine;
using Wacc;
using Wacc.CodeGen;
using Wacc.Emit;
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
            Console.Error.WriteLine("* Lexer");
            new Lexer(rts).Execute();
        }

        if (rts.DoParser || rts.DoAll)
        {
            Console.Error.WriteLine("* Parser");
            new Wacc.Parse.Parser(rts).Execute();
        }

        if (rts.DoCodeGen || rts.DoAll)
        {
            Console.Error.WriteLine("* CodeGen");
            new CodeGenerator(rts).Execute();
        }

        if (rts.DoCodeEmission || rts.DoAll)
        {
            Console.Error.WriteLine("* CodeEmit");
            new CodeEmitter(rts).Execute();
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine($"ERROR: {e.Message}");
        Environment.Exit(1);
    }
}