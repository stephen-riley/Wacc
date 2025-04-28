namespace Wacc;

public class Parser(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute() => Options.DoParser;
}