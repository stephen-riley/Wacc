using System.Diagnostics;

namespace Wacc.Exe;

public class GenExecutable(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute()
    {
        var tmpAsmFile = Path.GetTempFileName() + ".S";
        if (Options.Verbose)
        {
            Console.Error.WriteLine($"\nWriting to temp file {tmpAsmFile}");
        }
        File.WriteAllText(tmpAsmFile, Options.Assembly);
        var ps = Process.Start("gcc", $"{tmpAsmFile} -o {Options.OutputFile ?? Options.BaseFilename}");
        ps.WaitForExit();
        return ps.ExitCode == 0;
    }
}