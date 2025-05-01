namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute() => Options.DoCodeGen;
}