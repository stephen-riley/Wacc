using CommandLine;
using Wacc;

Parser.Default.ParseArguments<RuntimeState>(args)
    .WithParsed(rts =>
    {
        rts.Text = File.ReadAllText(rts.InputFile);
        new Driver(rts).Entrypoint();
    });
// .WithNotParsed(HandleParseError);