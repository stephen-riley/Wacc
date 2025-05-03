using System.Text;

namespace Wacc.Extensions;

public static class Extensions
{
    public static string X(this string s, int indent) => new StringBuilder().Insert(0, s, indent).ToString();
}