namespace Wacc.BookTestsDriver;

public record TestInfo(int Chapter, string Name, string Path, string Kind, int ExpectedExitCode = 0)
{
    public override string ToString() => $"chapter_{Chapter}/{Kind}/{Name} ({ExpectedExitCode})";
}