namespace Wacc.Exceptions;

public class TestDriverError : Exception
{
    public TestDriverError() { }

    public TestDriverError(string message) : base(message) { }

    public TestDriverError(string message, Exception inner) : base(message, inner) { }
}