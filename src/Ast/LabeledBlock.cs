using System.Text;
using Wacc.Tokens;

namespace Wacc.Ast;

public partial record LabeledBlock(Label Label, AstNode Stat) : AstNode
{
    public override bool IsBlockItem() => true;

    public new bool CanParse(Queue<Token> tokenStream) => Label.CanParse(tokenStream);

    public new static AstNode Parse(Queue<Token> tokenStream)
    {
        var label = Label.Parse(tokenStream);
        var stat = Block.Parse(tokenStream);
        return new LabeledBlock(label, stat);
    }

    public override string ToPrettyString(int indent = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("LabeledBlock(");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"label={Label.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent + 1)).AppendLine($"block={Stat.ToPrettyString(indent + 1)}");
        sb.Append(AstNode.IndentStr(indent)).Append(')');
        return sb.ToString();
    }

    public override IEnumerable<AstNode> Children() => [Label, Stat];
}