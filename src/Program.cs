using CommandLine;
using Wacc;

Console.Error.WriteLine("Wacc 1.0");

Parser.Default.ParseArguments<RuntimeState>(args)
    .WithParsed(rts =>
    {
        rts.Text = File.ReadAllText(rts.InputFile);
        new Driver(rts).Entrypoint();
    });
// .WithNotParsed(HandleParseError);