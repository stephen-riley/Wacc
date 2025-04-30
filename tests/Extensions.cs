using Wacc;

namespace Wacc.Tests;

public static class Extensions
{
    public static string ToTokenString(this List<Token> l) => string.Join(", ", l.Select(t => $"[{t}]"));
}