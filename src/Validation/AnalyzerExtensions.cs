using Wacc.Ast;

namespace Wacc.Validation;

public static class AnalyzerExtensions
{
    public static void Walk(this AstNode @this, Action<AstNode> callback, bool prefix = true, bool postfix = false)
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

    public static void WalkFor<T>(this AstNode @this, Action<T> callback, bool prefix = true, bool postfix = false)
        => @this.Walk(node =>
        {
            if (node is T t)
            {
                callback(t);
            }
        });

    public static void WalkFor(this AstNode @this, Func<AstNode, bool> pred, Action<AstNode> callback, bool prefix = true, bool postfix = false)
        => @this.Walk(node =>
        {
            if (pred(@this))
            {
                callback(@this);
            }
        });
}