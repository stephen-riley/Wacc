namespace Wacc.Exceptions;

public class LexerError : Exception
{
    public LexerError() { }

    public LexerError(string message) : base(message) { }

    public LexerError(string message, Exception inner) : base(message, inner) { }
}