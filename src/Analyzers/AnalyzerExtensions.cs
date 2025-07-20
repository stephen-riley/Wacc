using Wacc.Ast;

namespace Wacc.Analyzers;

public static class AnalyzerExtensions
{
    public static void Walk(this IAstNode @this, Action<IAstNode> callback, bool prefix = true, bool postfix = false)
    {
        if (prefix)
        {
            callback(@this);
        }
        foreach (var child in @this.Children())
        {
            child.Walk(callback, prefix, postfix);
        }
        if (postfix)
        {
            callback(@this);
        }
    }

    public static void WalkFor<T>(this IAstNode @this, Action<T> callback, bool prefix = true, bool postfix = false)
        => @this.Walk(node =>
        {
            if (node is T t)
            {
                callback(t);
            }
        });
}