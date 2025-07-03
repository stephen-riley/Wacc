using System.ComponentModel;
namespace Wacc.Tokens;

public enum TokenType
{
    [Description("=")] Assign,

    [Description("&")] BitwiseAnd,

    [Description("<<")] BitwiseLeft,

    [Description("|")] BitwiseOr,

    [Description(">>")] BitwiseRight,

    [Description("^")] BitwiseXor,

    [Description("}")] CloseBrace,

    [Description(")")] CloseParen,

    [Description("~")] Complement,

    [Description("")] Constant,

    [Description("--")] Decrement,

    [Description("/=")] DivAssign,

    [Description("/")] DivSign,

    [Description("==")] EqualTo,

    [Description(">=")] GreaterOrEqual,

    [Description(">")] GreaterThan,

    [Description("")] Identifier,

    [Description("int")] IntKw,

    [Description("<=")] LessOrEqual,

    [Description("<")] LessThan,

    [Description("&&")] LogicalAnd,

    [Description("!")] LogicalNot,

    [Description("||")] LogicalOr,

    [Description("-=")] MinusAssign,

    [Description("-")] MinusSign,

    [Description("%=")] ModAssign,

    [Description("%")] ModSign,

    [Description("*=")] MulAssign,

    [Description("*")] MulSign,

    [Description("!=")] NotEqualTo,

    [Description("{")] OpenBrace,

    [Description("(")] OpenParen,

    [Description("+=")] PlusAssign,

    [Description("+")] PlusSign,

    [Description("return")] ReturnKw,

    [Description(";")] Semicolon,

    [Description("void")] VoidKw,

    [Description("<WHITESPACE>")] WHITESPACE,

    [Description("<// COMMENT>")] COMMENT_SINGLE_LINE,

    [Description("</* COMMENT */>")] COMMENT_MULTI_LINE,

    [Description("<#DIRECTIVE>")] PREPROCESSOR_DIRECTIVE,

    [Description("<EOF>")] EOF
}