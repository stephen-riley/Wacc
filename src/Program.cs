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
            Console.Error.WriteLine($"\n{eName}: {e.Message}");
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