
namespace Wacc.CodeGen.AbstractAsm;

public record ProgramGen() : IAbstractAsm
{
    public void Emit(TextWriter stream) => stream.WriteLine(EmitString());

    public string EmitString() => "    .section __TEXT,__text,regular,pure_instructions";
}