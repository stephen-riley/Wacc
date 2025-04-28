using CommandLine;
using Wacc;

Console.Error.WriteLine("Wacc 1.0");

CommandLine.Parser.Default.ParseArguments<RuntimeState>(args)
    .WithParsed(opts =>
    {
        opts.Text = File.ReadAllText(opts.InputFile);

        new Lexer(opts).Execute();
        new Wacc.Parser(opts).Execute();
        new CodeGenerator(opts).Execute();
        new CodeEmitter(opts).Execute();
    });
// .WithNotParsed(HandleParseError);
