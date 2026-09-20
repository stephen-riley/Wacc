using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public class TackyGenerator(RuntimeState opts)
{
    internal static TacVar DUMMY = new("DUMMY");

    public RuntimeState Options = opts;

    internal List<TacFunction> functions = [];
    internal List<ITackyInstr> instructions = [];

    internal HashSet<string> ReservedLabels = ["_main"];

    internal Stack<string> BreakLabelStack = [];
    internal Stack<string> ContinueLabelStack = [];

    internal TacVar? LastTmpVar;
    internal int TmpVarCounter = 0;

    internal TacVar ReserveTmpVar()
    {
        if (functions.Count > 0)
        {
            var tv = new TacVar($"tmp.{TmpVarCounter++}");
            LastTmpVar = tv;
            functions[^1].Locals.Add(tv);
            return tv;
        }
        else
        {
            throw new TackyGenError("cannot allocate tmp var with no function declared");
        }
    }

    internal TacVar RegisterVar(TacVar tv)
    {
        functions[^1].Locals.Add(tv);
        return tv;
    }

    internal string GetCleanLabelName(string id) => ReservedLabels.Contains(id) ? $"{id}_X" : id;

    internal TacVar? GetLastTmpVar() => LastTmpVar;
    internal TacVar GetLastTmpVarOrFail() => GetLastTmpVar() ?? throw new TackyGenError("need to know last temp var in UnaryOp, but none available");

    internal int TmpLabelCounter = 0;
    internal string ReserveTmpLabel(string prefix = "_l") => $"{prefix}{TmpLabelCounter++}";

    internal void Emit(ITackyInstr instr)
    {
        instructions.Add(instr);
        // Console.Error.WriteLine($"> {instr}");
    }

    internal TacVal EmitWithDummy(ITackyInstr instr)
    {
        instructions.Add(instr);
        return DUMMY;
    }

    public bool Execute()
    {
        EmitTacky(Options.Ast);
        Options.Tacky = new TacProgram(functions);

        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("TAC IR:");
            Console.Error.WriteLine("=======");
        }

        if (!Options.Silent)
        {
            if (Options.Verbose || Options.OnlyThroughTacky)
            {
                var stream = Options.Verbose ? Console.Error : Console.Out;
                foreach (var f in Options.Tacky.Functions)
                {
                    foreach (var i in f.Instructions)
                    {
                        stream.WriteLine(i switch
                        {
                            TacConstant or TacVar => i.ToString(),
                            TacLabel l => $"\n{l.Identifier}:",
                            _ => $"    {i}"
                        });
                    }
                    stream.WriteLine();
                }
            }
        }

        return true;
    }

    internal TacVal EmitTacky(AstNode node)
    {
        try
        {
            return node.EmitTacky(this, node);
        }
        catch (NotImplementedException e)
        {
            throw new TackyGenError($"{GetType().Name}.{nameof(EmitTacky)} can't handle {node.GetType().Name} yet", e);
        }
    }
}