namespace Wacc.Tokens;

public record Token(TokenType TokenType, int Line, int Column, string? Str, int Int)
{
    public static Token EmptyToken = new(TokenType.EMPTY, 0, 0, "", -1);

    public bool MostlyEquals(Token t) => TokenType == t.TokenType && Line == t.Line && Column == t.Column && Str == t.Str;

    public override string ToString() => $"{TokenType}:{Str} ({Line}:{Column})";
}