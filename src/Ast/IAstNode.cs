using System.Diagnostics.CodeAnalysis;
using Wacc.Exceptions;
using Wacc.Tokens;

namespace Wacc.Ast;

public interface IAstNode
{
    public static IAstNode Parse(Queue<Token> tokenStream) => throw new ParseError();
}