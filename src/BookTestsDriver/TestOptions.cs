using CommandLine;

namespace Wacc.BookTestsDriver;

[Verb("test", HelpText = "Run book tests")]
public class TestOptions
{
    const string TestsPathEnvVar = "WACC_TESTS_PATH";

    [Option('c', "chapter", HelpText = "Run tests up through this chapter")]
    public int Chapter { get; set; } = 1;

    [Option('e', "extra-credit", HelpText = "Include extra credit tests")]
    public bool ExtraCredit { get; set; } = false;

    [Option("path", HelpText = "Path to book tests (or use WACC_TESTS_PATH env var)")]
    public string CliTestsPath { get; set; } = null!;

    [Option("no-invalid", HelpText = "Don't include invalid tests")]
    public bool NoInvalid { get; set; } = false;

    [Option('r', "run-from-failure", HelpText = "Start running tests at last failed test")]
    public bool RunFromFailure { get; set; } = false;

    [Option("stage", HelpText = "Run through stage")]
    public string Stage { get; set; } = "";

    [Option("verbose", HelpText = "Show list of tests before executing")]
    public bool Verbose { get; set; } = false;

    [Option('f', "fail-fast", HelpText = "Halt tests immediately on first failure")]
    public bool FailFast { get; set; } = false;

    [Option("failure-code", HelpText = "Expected exit code for failures")]
    public int FailureExitCode { get; set; } = -1;

    [Option("plan", HelpText = "Show tests to be run (implies --verbose), but don't run them")]
    public bool Plan { get; set; } = false;

    [Option('a', "all", HelpText = "Run all tests from the beginning")]
    public bool AllTests { get; set; } = false;

    public bool AllStages => Stage == "";

    public string TestsPath => CliTestsPath ?? Environment.GetEnvironmentVariable(TestsPathEnvVar) ?? throw new InvalidOperationException("no tests path specified");
}