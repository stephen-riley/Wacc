namespace Wacc;

public record Token(TokenType TokenType, int Index, string? Str, int Int)
{
    public override string ToString() => $"{TokenType}:{Str} ({Index})";
}