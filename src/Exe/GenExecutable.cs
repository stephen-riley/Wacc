using System.Diagnostics;

namespace Wacc.Exe;

public class GenExecutable(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute()
    {
        var tmpFileBase = Path.GetTempFileName();

        if (Options.Verbose)
        {
            Console.Error.WriteLine($"\nWriting to temp file {tmpFileBase}.S");
        }
        File.WriteAllText($"{tmpFileBase}.S", Options.Assembly);
        var assemble = Process.Start("gcc", $"-g -c -o {tmpFileBase}.o {tmpFileBase}.S");
        assemble.WaitForExit();
        var link = Process.Start("gcc", $"{tmpFileBase}.o -o {Options.OutputFile ?? Options.BaseFilename} -arch arm64");
        link.WaitForExit();
        return link.ExitCode == 0;
    }
}