using Wacc.Tacky.Instructions;

namespace Wacc.Tacky.Instruction

public record TacFunction(string Name, IEnumerable<ITackyInstr> Instructions) : ITackyInstr
{

}