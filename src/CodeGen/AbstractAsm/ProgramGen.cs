
namespace Wacc.CodeGen.AbstractAsm;

public record ProgramGen(IEnumerable<FunctionGen> Funcs) : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString() => string.Join('\n', Funcs.Select(f => f.EmitString()));
}