using CommandLine;
using Wacc;
using Wacc.CodeGen;
using Wacc.Emit;
using Wacc.Lex;

// Console.Error.WriteLine("Wacc 1.0");

CommandLine.Parser.Default.ParseArguments<RuntimeState>(args)
    .WithParsed(rts =>
    {
        rts.Text = File.ReadAllText(rts.InputFile);
        Entrypoint(rts);
    });
// .WithNotParsed(HandleParseError);

void Entrypoint(RuntimeState rts)
{
    try
    {
        new Lexer(rts).Execute();
        new Wacc.Parse.Parser(rts).Execute();
        new CodeGenerator(rts).Execute();
        new CodeEmitter(rts).Execute();
    }
    catch (Exception e)
    {
        Console.Error.WriteLine($"ERROR: {e.Message}");
        Environment.Exit(-1);
    }
}