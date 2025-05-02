namespace Wacc.Tokens;

public enum TokenType
{
    Identifier,
    Constant,
    IntKw,
    VoidKw,
    ReturnKw,
    OpenParen,
    CloseParen,
    OpenBrace,
    CloseBrace,
    Semicolon,
    WHITESPACE,
    COMMENT_SINGLE_LINE,
    COMMENT_MULTI_LINE,
    EOF
}