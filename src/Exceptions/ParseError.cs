namespace Wacc.Exceptions;

public class ParseError : Exception
{
    public ParseError() { }

    public ParseError(string message) : base(message) { }

    public ParseError(string message, Exception inner) : base(message, inner) { }
}