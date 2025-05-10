namespace Wacc.Exceptions;

public class TackyGenError : Exception
{
    public TackyGenError() { }

    public TackyGenError(string message) : base(message) { }

    public TackyGenError(string message, Exception inner) : base(message, inner) { }
}