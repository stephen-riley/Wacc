using Wacc.Ast;
using Wacc.Exceptions;
using VarMap = System.Collections.Generic.Dictionary<string, string>;

namespace Wacc.Analyzers;

public record SemanticAnalyzer(RuntimeState Options)
{
    internal VarMap VariableMap = [];

    internal Dictionary<string, int> UniqueVarCounters = [];

    public bool Execute()
    {
        Options.Ast = Validate(Options.Ast);

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

        return true;
    }

    public Ast.Program Validate(IAstNode ast)
    {
        if (ast is Ast.Program program)
        {
            program = new VarAnalyzer(Options).Validate(program);
            program = LabelAnalyzer.Validate(program);
            return program;
        }
        else
        {
            throw new ValidationError($"Top AST node must be Program, not {ast.GetType().Name}");
        }
    }
}