using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public record LabeledStatement(Label Label, IAstNode Stat) : IAstNode
{
    public bool IsBlockItem() => true;

    public static bool CanParse(Queue<Token> tokenStream) => Label.CanParse(tokenStream);

    public static IAstNode Parse(Queue<Token> tokenStream)
    {
        var label = Label.Parse(tokenStream);
        var stat = Statement.Parse(tokenStream, nested: true);
        return new LabeledStatement(label, stat);
    }

    public string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("LabeledStatement(");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"label={Label.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent + 1)).AppendLine($"stat={Stat.ToPrettyString(indent + 1)}");
        sb.Append(IAstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public IEnumerable<IAstNode> Children() => [Label, Stat];
}