using System.ComponentModel;
namespace Wacc.Tokens;

public enum TokenType
{
    [Description("=")] Assign,

    [Description("&=")] CompoundBitwiseAnd,

    [Description("|=")] CompoundBitwiseOr,

    [Description("^=")] CompoundBitwiseXor,

    [Description("<<=")] CompoundBitwiseLeft,

    [Description(">>=")] CompoundBitwiseRight,

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

    [Description("/=")] CompoundDiv,

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

    [Description("-=")] CompoundMinus,

    [Description("-")] MinusSign,

    [Description("%=")] CompoundMod,

    [Description("%")] ModSign,

    [Description("*=")] CompoundMul,

    [Description("*")] MulSign,

    [Description("!=")] NotEqualTo,

    [Description("{")] OpenBrace,

    [Description("(")] OpenParen,

    [Description("+=")] CompoundPlus,

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