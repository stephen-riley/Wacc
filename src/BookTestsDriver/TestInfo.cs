namespace Wacc.BookTestsDriver;

public record TestInfo(int Chapter, string Name, string Path, string Kind, int ExpectedExitCode = 0)
{
    public class KindComparer : IComparer<TestInfo>
    {
        public int Compare(TestInfo? x, TestInfo? y)
            => x is null || y is null
                ? throw new InvalidOperationException($"")
                : CompareImpl(x, y);

        public int CompareImpl(TestInfo x, TestInfo y)
        {
            if (x.Kind == y.Kind) return 0;
            if (x.Kind == "valid") return -1;
            if (y.Kind == "valid") return 1;
            return string.Compare(x.Kind, y.Kind);
        }
    }

    public bool IsValid => Kind.Contains("valid") && !IsInvalid;
    public bool IsInvalid => Kind.Contains("invalid");
    public bool IsExtraCredit => Kind.Contains("extra_credit");
    public bool IsLex => Kind.Contains("_lex");
    public bool IsParse => Kind.Contains("_parse");
    public bool IsSemantics => Kind.Contains("_semantics");

    public override string ToString()
    {
        var s = $"chapter_{Chapter}/{Kind}/{Name}";
        if (IsValid) s += $" ({ExpectedExitCode})";
        return s;
    }
}