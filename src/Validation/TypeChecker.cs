using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Validation.Tools;
using Wacc.Validation.Types;

namespace Wacc.Validation;

public class TypeChecker
{
    public static CompUnit Validate(CompUnit program)
    {
        program.WalkFor<FunctionCall>(fc =>
        {
            var parentIm = fc.IdentifierMap?.Parent;
            if (parentIm is not null)
            {
                if (parentIm.TryGetValue(fc.Identifier.Name, out var resolvedName, out _))
                {
                    switch (resolvedName.Type)
                    {
                        case FuncType ft:
                            if (ft.ParamCount != fc.Args.Params.Count())
                            {
                                throw new TypeError($"Function {fc.Identifier.Name} expects {ft.ParamCount} arguments, but {fc.Args.Params.Count()} {(fc.Args.Params.Count() > 1 ? "were" : "was")} provided");
                            }
                            break;
                        default:
                            throw new TypeError($"Symbol {fc.Identifier.Name} was declared as {resolvedName.Type}");
                    }
                }
                else
                {
                    throw new TypeError($"Undeclared function {fc.Identifier.Name}");
                }

            }
        });

        return program;
    }
}