using System.Text;
using Wacc.Tokens;

namespace Wacc.Extensions;

public static class Ext
{
    public static string X(this string s, int indent) => new StringBuilder().Insert(0, s, indent).ToString();

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> callback)
    {
        foreach (var el in list)
        {
            callback(el);
        }
    }

    public static T Do<T>(Func<T> f) => f();

    public static string NextFewTokens(this Queue<Token> tokenStream, int count = 3)
        => string.Join(',', tokenStream.Take(count).Select(t => t.ToString()));
}