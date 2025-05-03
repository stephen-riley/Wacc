namespace Wacc.Emit;

public class CodeEmitter(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute()
    {
        foreach (var i in Options.AbstractInstructions)
        {
            i.Emit(Console.Out);
        }

        return true;
    }
}