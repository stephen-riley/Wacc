namespace Wacc.Tokens;

public record Token(TokenType TokenType, int Line, int Index, string? Str, int Int)
{
    public bool MostlyEquals(Token t) => TokenType == t.TokenType && Line == t.Line && Index == t.Index && Str == t.Str;

    public override string ToString() => $"{TokenType}:{Str} ({Line}:{Index})";
}