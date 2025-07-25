using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Ast;

public interface IAstNode
{
    const string INDENT = "  ";

    public bool IsBlockItem() => false;

    public static string IndentStr(int indent = 0) => INDENT.X(indent);

    public static bool CanParse(Queue<Token> tokenStream) => throw new NotImplementedException();

    public static IAstNode Parse(Queue<Token> tokenStream) => throw new ParseError();

    public void PrettyPrint(TextWriter stream, int indent = 0) => stream.Write(ToPrettyString(indent));

    public string ToPrettyString(int indent = 0);

    public IEnumerable<IAstNode> Children();
}