using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.TackyGen;

public class TackyGenerator(RuntimeState opts)
{
    public RuntimeState Options = opts;

    internal List<TacFunction> functions = [];
    internal List<ITackyInstr> instructions = [];

    internal int TmpCounter = 0;
    internal TacVar ReserveTmpVar() => new($"tmp.{TmpCounter++}");
    internal TacVar? GetLastTmpVar() => instructions[^1].GetDst();

    public bool Execute()
    {
        EmitTacky(Options.Ast);
        Options.Tacky = new TacProgram(functions);

        if (Options.Verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("TACKY:");
        }
        if (Options.Verbose || Options.OnlyThroughTacky)
        {
            var stream = Options.Verbose ? Console.Error : Console.Out;
            foreach (var f in Options.Tacky.Functions)
            {
                foreach (var i in f.Instructions)
                {
                    stream.WriteLine(i);
                }
                stream.WriteLine();
            }
        }

        return true;
    }

    internal void EmitTacky(IAstNode node)
    {
        switch (node)
        {
            case Ast.Program p:
                foreach (var s in p.Statements)
                {
                    EmitTacky(s);
                }
                break;

            case Function f:
                instructions = [];
                EmitTacky(f.Body);
                functions.Add(new TacFunction(f.Name, instructions));
                break;

            case Return r:
                if (r.Expr is Constant literal)
                {
                    instructions.Add(new TacReturn(new TacConstant(literal.Int)));
                }
                else
                {
                    EmitTacky(r.Expr);
                    instructions.Add(new TacReturn(GetLastTmpVar() ?? throw new TackyGenError("need to know last temp var in Return, but none available")));
                }
                break;

            case Constant c:
                instructions.Add(new TacConstant(c.Int));
                break;

            case UnaryOp u:
                var src = default(TacVal);
                if (u.Expr is Constant uc)
                {
                    src = new TacConstant(uc.Int);
                }
                else
                {
                    EmitTacky(u.Expr);
                    src = GetLastTmpVar() ?? throw new TackyGenError("need to know last temp var in UnaryOp, but none available");
                }
                var dst = ReserveTmpVar();
                instructions.Add(new TacUnary(u.Op, src, dst));
                break;

            default:
                throw new NotImplementedException($"{nameof(TackyGenerator)}.{nameof(EmitTacky)} can't handle {node.GetType()} yet");
        }
    }
}