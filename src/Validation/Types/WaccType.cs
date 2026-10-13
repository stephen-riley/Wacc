namespace Wacc.Validation.Types;

public readonly union WaccType(IntType, FuncType)
{
    public override string ToString() => this switch
    {
        IntType i => i.ToString(),
        FuncType f => f.ToString(),
    };
}