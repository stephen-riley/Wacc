using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options = opts;

    internal List<ITackyInstr> instructions = [];

    public bool Execute()
    {
        Walk(Options.Ast);
        Options.AbstractInstructions = instructions;

        return true;
    }

    internal List<ITackyInstr> Walk(IAstNode node)
    {
        switch (node)
        {
            case Ast.Program p:
                instructions.Add(new TacProgram());
                foreach (var f in p.Statements)
                {
                    instructions.AddRange(Walk(f));
                }
                return [];

            case Function f:
                var body = Walk(f.Body);
                instructions.Add(new FunctionGen(f.Name, body));
                return [];

            case Return r:
                var expr = Walk(r.Expr);
                // TO DO: that MoveGen will get fixed in the future
                return [.. expr,
                    new MovGen(new OperandRegGen(Register.W0), new OperandRegGen(Register.W10)),
                    new RetGen()
                ];

            case Constant c:
                return [new ImmGen(c.Int)];

            case UnaryOp u:
                return u.Op switch
                {
                    "-" => [new NegGen(new OperandRegGen(Register.W10), new OperandRegGen(Register.W10))],
                    "~" => [new MvnGen(new OperandRegGen(Register.W10), new OperandRegGen(Register.W10))],
                    _ => throw new CodeGenError($"invalid UnaryOp '{u.Op}'"),
                };

            default:
                throw new NotImplementedException($"{nameof(CodeGenerator)}.{nameof(Walk)} can't handle {node.GetType()} yet");
        }
    }
}