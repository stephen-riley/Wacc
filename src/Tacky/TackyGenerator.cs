using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.Tacky;

public partial class TackyGenerator(RuntimeState opts)
{
    internal static TacVar DUMMY = new("DUMMY");

    public RuntimeState Options = opts;

    internal List<TacFunction> functions = [];
    internal List<ITackyInstr> instructions = [];

    internal HashSet<string> ReservedLabels = ["_main"];

    internal Stack<string> BreakLabelStack = [];
    internal Stack<string> ContinueLabelStack = [];
    internal Stack<SwitchContext> SwitchContextStack = [];

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

    internal Dictionary<string, int> TmpLabelCounters = [];
    internal string ReserveTmpLabel(string prefix = "_l")
    {
        if (TmpLabelCounters.TryGetValue(prefix, out var counter))
        {
            var label = $"{prefix}{counter}";
            TmpLabelCounters[prefix] = counter + 1;
            return label;
        }
        else
        {
            TmpLabelCounters[prefix] = 1;
            return $"{prefix}0";
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

    internal void EmitLabel(string label) => Emit(new TacLabel(label));

    internal void EmitLabelAndGetNext(ref string current, string prefix)
    {
        EmitLabel(current);
        current = ReserveTmpLabel(prefix);
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

    internal TacVal EmitTacky(AstNode node) => node switch
    {
        Assignment a => EmityTackyForAssignment(a),
        BinaryOp bo => EmityTackyForBinaryOp(bo),
        Block b => EmityTackyForBlock(b),
        BlockItem bi => EmityTackyForBlockItem(bi),
        Break b => EmityTackyForBreak(b),
        Case c => EmityTackyForCase(c),
        CompUnit cu => EmityTackyForCompUnit(cu),
        Constant c => EmityTackyForConstant(c),
        Continue c => EmityTackyForContinue(c),
        Declaration d => EmityTackyForDeclaration(d),
        Default d => EmityTackyForDefault(d),
        DoLoop dl => EmityTackyForDoLoop(dl),
        Expression e => EmityTackyForExpression(e),
        Factor f => EmityTackyForFactor(f),
        ForInit fi => EmityTackyForForInit(fi),
        ForLoop fl => EmityTackyForForLoop(fl),
        Function f => EmityTackyForFunction(f),
        Goto g => EmityTackyForGoto(g),
        IfElse ie => EmityTackyForIfElse(ie),
        Label l => EmityTackyForLabel(l),
        LabeledBlock lb => EmityTackyForLabeledBlock(lb),
        NullStatement ns => EmityTackyForNullStatement(ns),
        OpAssignment oa => EmityTackyForOpAssignment(oa),
        PostfixOp po => EmityTackyForPostfixOp(po),
        PrefixOp po => EmityTackyForPrefixOp(po),
        Return r => EmityTackyForReturn(r),
        Statement s => EmityTackyForStatement(s),
        Switch s => EmityTackyForSwitch(s),
        Ternary t => EmityTackyForTernary(t),
        UnaryOp uo => EmityTackyForUnaryOp(uo),
        Var v => EmityTackyForVar(v),
        WhileLoop wl => EmityTackyForWhileLoop(wl),
    };
}