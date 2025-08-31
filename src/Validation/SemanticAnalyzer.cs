using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

public record SemanticAnalyzer(RuntimeState Options)
{
    internal Dictionary<string, int> UniqueVarCounters = [];

    public bool Execute()
    {
        Options.Ast = Validate(Options.Ast);

        if (!Options.Silent)
        {
            if (Options.Verbose || Options.OnlyThroughLexer)
            {
                if (Options.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("SEMANTIC VALIDATION:");
                    Console.Error.WriteLine("=====================");

                    var stream = Options.Verbose ? Console.Error : Console.Out;
                    stream.WriteLine(Options.Ast.ToPrettyString());
                }
            }
        }

        return true;
    }

    public static CompUnit Validate(IAstNode ast)
    {
        if (ast is CompUnit program)
        {
            program = new VarAnalyzer().Validate(program);
            program = new LoopAnalyzer().Validate(program);
            program = LabelAnalyzer.Validate(program);
            return program;
        }
        else
        {
            throw new ValidationError($"Top AST node must be Program, not {ast.GetType().Name}");
        }
    }
}