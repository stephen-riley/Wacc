namespace Wacc.Tokens;

public enum TokenType
{
    BitwiseAnd,
    BitwiseLeft,
    BitwiseOr,
    BitwiseRight,
    BitwiseXor,
    CloseBrace,
    CloseParen,
    Complement,
    Constant,
    Decrement,
    Identifier,
    IntKw,
    MinusSign,
    PlusSign,
    MulSign,
    DivSign,
    ModSign,
    OpenBrace,
    OpenParen,
    ReturnKw,
    Semicolon,
    VoidKw,
    WHITESPACE,
    COMMENT_SINGLE_LINE,
    COMMENT_MULTI_LINE,
    PREPROCESSOR_DIRECTIVE,
    EOF
}