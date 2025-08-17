using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Wacc.Exceptions;

namespace Wacc.BookTestsDriver;

public class TestDriver(TestOptions rts)
{
    public TestOptions Options { get; init; } = rts;

    internal string TestsPath = null!;

    internal List<TestInfo> Tests = [];

    public List<string> Errors = [];

    public List<string> SuccessfulTests = [];

    public void Entrypoint()
    {
        TestsPath = ResolveHomeRelativePath(Options.TestsPath);

        CollectValidTests();
        CollectOtherTests();

        if (!Options.AllTests)
        {
            FilterOutSuccessfulTests();
        }

        if (Options.Verbose || Options.Plan)
        {
            Tests.ForEach(Console.Error.WriteLine);
        }

        if (Options.Plan)
        {
            Console.Error.WriteLine("----------------------------------------------------------------------");
            Console.Error.WriteLine($"Planned {Tests.Count} tests");
            Console.Error.WriteLine();
            Console.Error.WriteLine("OK");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        RunTests();
        stopwatch.Stop();

        SaveSuccessfulTests();

        Console.Error.WriteLine();
        Console.Error.WriteLine("----------------------------------------------------------------------");
        Console.Error.WriteLine($"Ran {Tests.Count} tests in {double.Truncate(stopwatch.ElapsedMilliseconds / 10) / 100}s");
        Console.Error.WriteLine();

        if (Errors.Count == 0)
        {
            Console.Error.WriteLine("OK");
        }
        else
        {
            Console.Error.WriteLine($"FAILED (failures={Errors.Count})");
            Errors.ForEach(Console.Error.WriteLine);
        }
    }

    void CollectValidTests()
    {
        var path = Path.Combine(TestsPath, "expected_results.json");
        var doc = JsonNode.Parse(File.ReadAllText(path)) ?? throw new FileNotFoundException($"Cannot find expected_results.json at {path}");

        foreach (var test in doc.AsObject())
        {
            var fullPath = ResolveHomeRelativePath(Path.Combine(Options.TestsPath, "tests", test.Key));
            if (TryParseBookTestName(fullPath, test.Value, out var ti))
            {
                if (!ti.IsExtraCredit || Options.ExtraCredit)
                {
                    Tests.Add(ti);
                }
            }
        }
    }

    void CollectOtherTests()
    {
        var testsPath = ResolveHomeRelativePath(Path.Combine(Options.TestsPath, "tests"));
        var allFiles = Directory.GetFiles(testsPath, "*.c", SearchOption.AllDirectories);
        var filtered = allFiles.Where(f => !f.Contains("/valid/"))
        .Select(f =>
        {
            TryParseBookTestName(f, Options.FailureExitCode, out var testInfo);
            return testInfo;
        })
        .OfType<TestInfo>()
        .Where(t =>
        {
            var okay = true;
            okay = okay && t.Chapter <= Options.Chapter;
            okay = okay && (t.IsExtraCredit ? Options.ExtraCredit : true);
            okay = okay && (t.IsInvalid ? !Options.NoInvalid : true);
            okay = okay && (t.IsLex && !Options.AllStages ? Options.Stage == "lex" : true);
            okay = okay && (t.IsParse && !Options.AllStages ? Options.Stage == "parse" : true);
            okay = okay && (t.IsSemantics && !Options.AllStages ? Options.Stage == "semantics" : true);
            return okay;
        });

        Tests.AddRange(filtered);
        Tests = SortTests();
    }

    List<TestInfo> SortTests()
    {
        return [.. Tests.OrderBy(t => t.Chapter).ThenBy(t => t, new TestInfo.KindComparer()).ThenBy(t => t.Name)];
    }

    void RunTests()
    {
        foreach (var test in Tests)
        {
            var success = RunTest(test, out var errorMessage);
            Console.Error.Write(success ? '.' : 'F');
            if (success)
            {
                SuccessfulTests.Add(test.Path);
            }
            else
            {
                Errors.Add($"{test.Path}\n   {errorMessage}\n");
                if (Options.FailFast)
                {
                    break;
                }
            }
        }
    }

    public bool RunTest(TestInfo test, [NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;

        var rts = new RuntimeState()
        {
            InputFile = test.Path,
            Text = File.ReadAllText(test.Path),
            OutputFile = "/tmp/a.out",
            AsmFilename = "/tmp/a.S",
            Silent = true,
        };

        SetRuntimeStateStage(rts, Options.Stage);

        try
        {
            new Driver(rts).Entrypoint();
            if (Options.Stage == "")
            {
                var actualCode = RunProgram("/tmp/a.out");
                if (actualCode == test.ExpectedExitCode)
                {
                    return true;
                }
                else
                {
                    errorMessage = $"expected exit code {test.ExpectedExitCode}, got {actualCode}";
                    return false;
                }
            }
            else
            {
                // safe to do this since any failure from the Driver will result in an exception
                return true;
            }
        }
        catch (Exception e)
        {
            if (test.IsValid)
            {
                errorMessage = $"{e.GetType().Name}: {e.Message}";
                return false;
            }
            else
            {
                return true;
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
            default: throw new TestDriverError($"invalid --stage {stage}");
        }
    }

    bool TryParseBookTestName(string jsonTestName, JsonNode? jsonValue, [NotNullWhen(true)] out TestInfo? test)
    {
        test = null;

        if (jsonValue is null) return false;

        var exitCode = (int)(jsonValue.AsObject()["return_code"] ?? throw new NullReferenceException());

        return TryParseBookTestName(jsonTestName, exitCode, out test);
    }

    bool TryParseBookTestName(string fullPath, int expectedExitCode, [NotNullWhen(true)] out TestInfo? test)
    {
        test = null;

        var partialPath = fullPath.Replace(Path.Combine(ResolveHomeRelativePath(Options.TestsPath), "tests"), "");
        var pieces = partialPath.Split('/').Where(p => p != "").ToArray();

        var chapter = int.Parse(pieces[0].Replace("chapter_", ""));
        if (chapter > Options.Chapter)
        {
            return false;
        }

        var name = pieces[^1];
        var kind = string.Join('/', pieces[1..^1]);

        test = new TestInfo(chapter, name, fullPath, kind, expectedExitCode);
        return true;
    }

    void SaveSuccessfulTests()
    {
        var saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".wacctests");
        File.WriteAllLines(saveFile, SuccessfulTests);
    }

    void FilterOutSuccessfulTests()
    {
        var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".wacctests");

        if (File.Exists(filename))
        {
            SuccessfulTests = [.. File.ReadAllLines(filename)];
            var pastTests = SuccessfulTests.ToHashSet();
            Tests = [.. Tests.Where(t => !pastTests.Contains(t.Path))];
        }
    }
}