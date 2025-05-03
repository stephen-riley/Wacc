using Wacc.Ast;
using Wacc.CodeGen.AbstractAsm;

namespace Wacc.CodeGen;

public class CodeGenerator(RuntimeState opts)
{
    public RuntimeState Options = opts;

    internal List<IAbstractAsm> instructions = [];

    public bool Execute()
    {
        Walk(Options.Ast);
        Options.AbstractInstructions = instructions;

        return true;
    }

    internal List<IAbstractAsm> Walk(IAstNode node)
    {
        switch (node)
        {
            case Ast.Program p:
                instructions.Add(new ProgramGen());
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
                // TODO: that MoveGen will get fixed in the future
                return [
                    new MovGen(new OperandRegGen(Register.W0),expr.ToArray()[0]),
                    new RetGen()
                ];

            case Constant c:
                return [new ImmGen(c.Int)];

            default:
                throw new NotImplementedException($"{nameof(CodeGenerator)}.{nameof(Walk)} can't handle {node.GetType()} yet");
        }
    }

    internal static T Do<T>(Func<T> f) => f();
}