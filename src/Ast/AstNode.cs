using Wacc.Exceptions;
using Wacc.Extensions;
using Wacc.Tokens;

namespace Wacc.Ast;

public closed record AstNode
{
    protected static readonly string INDENT = "  ";

    public virtual bool IsBlockItem() => false;

    public static string IndentStr(int indent = 0) => INDENT.X(indent);

    public static bool CanParse(Queue<Token> tokenStream) => throw new NotImplementedException();

    public static AstNode Parse(Queue<Token> tokenStream) => throw new ParseError();

    public virtual string ToPrettyString(int indent = 0) => throw new NotImplementedException($"{GetType().Name}.{nameof(ToPrettyString)}");

    public virtual IEnumerable<AstNode> Children() => [];
}