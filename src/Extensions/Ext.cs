using System.Text;

namespace Wacc.Extensions;

public static class Ext
{
    public static string X(this string s, int indent) => new StringBuilder().Insert(0, s, indent).ToString();

    public static T Do<T>(Func<T> f) => f();
}