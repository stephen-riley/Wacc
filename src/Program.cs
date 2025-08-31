using CommandLine;
using Wacc;
using Wacc.BookTestsDriver;

Parser.Default.ParseArguments<RuntimeState, TestOptions>(args)
    .WithParsed<RuntimeState>(rts =>
    {
        try
        {
            rts.Text = File.ReadAllText(rts.InputFile);
            new Driver(rts).Entrypoint();
        }
        catch (Exception e)
        {
            var eName = e.GetType().Name;
            var className = e.TargetSite?.DeclaringType?.Name ?? "{UNKNOWN THROWER}";
            var methodName = e.TargetSite?.Name ?? "{UNKNOWN METHOD}";
            Console.Error.WriteLine($"\n[{className}.{methodName}] {eName}: {e.Message}");
            if (!eName.EndsWith("Error"))
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            Environment.Exit(-1);
        }
    })
    .WithParsed<TestOptions>(testOptions =>
    {
        new TestDriver(testOptions).Entrypoint();
    });
// .WithNotParsed(HandleParseError);