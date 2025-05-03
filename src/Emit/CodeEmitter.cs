using System.Text;

namespace Wacc.Emit;

public class CodeEmitter(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute()
    {
        Emit(Console.Out);
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        Emit(sw);
        Options.Assembly = sb.ToString();

        return true;
    }

    internal void Emit(TextWriter stream)
    {
        stream.WriteLine();
        foreach (var i in Options.AbstractInstructions)
        {
            i.Emit(stream);
        }
    }
}