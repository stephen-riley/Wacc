using Wacc.Tacky.Instructions;

namespace Wacc.Tacky.Instruction

public record TacProgram(IEnumerable<TacFunction> Functions) : ITackyInstr
{

}