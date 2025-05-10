namespace Wacc.Exceptions;

public class CodeGenError : Exception
{
    public CodeGenError() { }

    public CodeGenError(string message) : base(message) { }

    public CodeGenError(string message, Exception inner) : base(message, inner) { }
}