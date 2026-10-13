namespace Wacc.Validation.Types;

public record FuncType(int ParamCount)
{
    public override string ToString() => $"FuncType({ParamCount})";
}