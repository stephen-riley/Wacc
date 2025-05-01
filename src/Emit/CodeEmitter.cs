namespace Wacc.Emit;

public class CodeEmitter(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute() => Options.DoCodeEmission;
}