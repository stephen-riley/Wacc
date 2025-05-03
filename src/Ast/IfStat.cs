using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Ast;

public class IfStat : IAstNode
{
    public static IfStat Parse(Queue<Token> tokenStream) { return null!; }

    public string ToPrettyString(int indent = 0)
        => IAstNode.INDENT.X(indent) + "IfStat(...)";
}