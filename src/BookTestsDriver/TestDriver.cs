using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Wacc.BookTestsDriver;

public class TestDriver(TestRuntimeState rts)
{
    public TestRuntimeState Trts { get; init; } = rts;

    internal string TestsPath = null!;

    internal List<TestInfo> Tests = [];

    public void Entrypoint()
    {
        TestsPath = ResolveHomeRelativePath(Trts.TestsPath);

        CollectValidTests();
        SortTests();
        RunTests();

        // for (var chapter = 1; chapter <= Trts.Chapter; chapter++)
        // {
        //     RunTests(chapter, "valid");

        //     RunTests(chapter, "invalid_lex", !Trts.NoInvalid);
        //     RunTests(chapter, "invalid_parse", !Trts.NoInvalid);
        //     RunTests(chapter, "invalid_semantics", !Trts.NoInvalid);

        //     RunTests(chapter, "valid/extra_credit", Trts.ExtraCredit);
        //     RunTests(chapter, "invalid_lex/extra_credit", !Trts.NoInvalid && Trts.ExtraCredit);
        //     RunTests(chapter, "invalid_parse/extra_credit", !Trts.NoInvalid && Trts.ExtraCredit);
        //     RunTests(chapter, "invalid_semantics/extra_credit", !Trts.NoInvalid && Trts.ExtraCredit);
        // }

        Console.Error.WriteLine();
        Console.Error.WriteLine("PASSED.");
    }

    void CollectValidTests()
    {
        var path = Path.Combine(TestsPath, "expected_results.json");
        var doc = JsonNode.Parse(File.ReadAllText(path)) ?? throw new FileNotFoundException($"Cannot find expected_results.json at {path}");

        foreach (var test in doc.AsObject())
        {
            var pieces = test.Key.Split('/');

            var chapter = int.Parse(pieces[0].Replace("chapter_", ""));
            if (chapter > Trts.Chapter) continue;

            var name = pieces[^1];
            var kind = string.Join('/', pieces[1..^1]);

            if (test.Value is not null)
            {
                var code = (int)(test.Value!["return_code"] ?? throw new NullReferenceException());
                var fullPath = Path.Combine(TestsPath, "tests", test.Key);
                Tests.Add(new TestInfo(chapter, name, fullPath, kind, code));
            }
        }
    }

    void SortTests()
    {
        Tests = [.. Tests.OrderBy(t => t.Chapter).ThenBy(t => t.Kind).ThenBy(t => t.Name)];
    }

    void RunTests()
    {
        Console.Error.WriteLine($"Running {Tests.Count} tests.");
        foreach (var test in Tests)
        {
            RunTest(test);
        }
    }

    public void RunTest(TestInfo test)
    {
        var rts = new RuntimeState()
        {
            InputFile = test.Path,
            Text = File.ReadAllText(test.Path),
            OutputFile = "/tmp/a.out",
            AsmFilename = "/tmp/a.S"
        };

        SetRuntimeStateStage(rts, Trts.Stage);

        try
        {
            new Driver(rts).Entrypoint();
            if (Trts.Stage == "")
            {
                var actualCode = RunProgram("/tmp/a.out");
                if (actualCode != test.ExpectedExitCode)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine($"ERROR in {test.Path}: expected exit code {test.ExpectedExitCode}, got {actualCode}");
                    return;
                }
            }
            Console.Error.Write('.');
        }
        catch (Exception e)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"ERROR in {test.Path}: {e.Message}");
            Environment.Exit(-1);
        }
    }

    public void RunTests(int ch, string kind, bool execute = true)
    {
        if (!execute) return;

        var relativePath = Path.Combine(Trts.TestsPath, "tests", $"chapter_{ch}", kind);
        var path = ResolveHomeRelativePath(relativePath);

        if (!Directory.Exists(path)) return;

        foreach (var fname in Directory.GetFiles(path).Order())
        {
            var rts = new RuntimeState()
            {
                InputFile = fname,
                Text = File.ReadAllText(fname),
                OutputFile = "/tmp/a.out",
                AsmFilename = "/tmp/a.S"
            };

            SetRuntimeStateStage(rts, Trts.Stage);

            try
            {
                new Driver(rts).Entrypoint();
                if (Trts.Stage == "")
                {
                    RunProgram("/tmp/a.out");
                }
                Console.Error.Write('.');
            }
            catch (Exception e)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine($"ERROR in chapter_{ch}/{kind}/{Path.GetFileName(fname)}: {e.Message}");
                Environment.Exit(-1);
            }
        }
    }

    public static int RunProgram(string path)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException($"could not start process {path}");
        process.WaitForExit();
        return process.ExitCode;
    }

    public static string ResolveHomeRelativePath(string path)
    {
        if (path.StartsWith('~'))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path[2..]);
        }
        return path;
    }

    public static void SetRuntimeStateStage(RuntimeState rts, string stage)
    {
        switch (stage)
        {
            case "lex": rts.OnlyThroughLexer = true; break;
            case "parse": rts.OnlyThroughParser = true; break;
            case "validate": rts.OnlyThroughValidate = true; break;
            case "tacky": rts.OnlyThroughTacky = true; break;
            case "codegen": rts.OnlyThroughCodeGen = true; break;
            case "emit": rts.OnlyThroughCodeEmit = true; break;
        }
    }
}