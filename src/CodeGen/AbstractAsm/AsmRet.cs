
namespace Wacc.CodeGen.AbstractAsm;

public record AsmRet : IAbstractAsm
{
    public string EmitString() => "    Ret";
}