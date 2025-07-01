namespace Wacc.Exceptions;

public class ValidationError : Exception
{
    public ValidationError() { }

    public ValidationError(string message) : base(message) { }

    public ValidationError(string message, Exception inner) : base(message, inner) { }
}