using Wacc.Ast;
using Wacc.Exceptions;

namespace Wacc.Validation;

public record LabelAnalyzer(RuntimeState Options)
{
    public static Ast.Program Validate(IAstNode ast)
    {
        if (ast is Ast.Program program)
        {
            var labels = new HashSet<string>();
            var gotoDestinations = new HashSet<string>();

            program.WalkFor<LabeledStatement>(ls =>
            {
                if (labels.Contains(ls.Label.Name))
                {
                    throw new ValidationError($"Label {ls.Label.Name} cannot appear more than once per file.");
                }
                else
                {
                    labels.Add(ls.Label.Name);
                }
            });

            program.WalkFor<Goto>(g =>
            {
                if (!labels.Contains(g.Label.Name))
                {
                    throw new ValidationError($"No label declared for goto {g.Label.Name}.");
                }
            });

            return program;
        }
        else
        {
            throw new ValidationError($"Top AST node must be Program, not {ast.GetType().Name}");
        }
    }
}