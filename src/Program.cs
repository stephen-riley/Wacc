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
            Console.Error.Write("lex ");
            new Lexer(rts).Execute();
        }

        if (rts.DoParser || rts.DoAll)
        {
            Console.Error.Write("parse ");
            new Wacc.Parse.Parser(rts).Execute();
        }

        if (rts.DoTacky || rts.DoAll)
        {
            Console.Error.Write("tacky ");
            new Wacc.TackyGen.TackyGenerator(rts).Execute();
        }

        if (rts.DoCodeGen || rts.DoAll)
        {
            Console.Error.Write("gen ");
            throw new NotImplementedException();
        }

        if (rts.DoCodeEmission || rts.DoAll)
        {
            Console.Error.Write("emit ");
            new CodeEmitter(rts).Execute();
        }

        // TODO: -S should stop here

        if (rts.DoAll)
        {
            Console.Error.Write("assemble ");
            new GenExecutable(rts).Execute();
        }

        Console.Error.WriteLine();
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