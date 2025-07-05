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

    [Description("&=")] CompoundBitwiseAnd,

    [Description("|=")] CompoundBitwiseOr,

    [Description("^=")] CompoundBitwiseXor,

    [Description("<<=")] CompoundBitwiseLeft,

    [Description(">>=")] CompoundBitwiseRight,

    [Description("/=")] CompoundDiv,

    [Description("-=")] CompoundMinus,

    [Description("%=")] CompoundMod,

    [Description("*=")] CompoundMul,

    [Description("+=")] CompoundPlus,

    [Description("}")] CloseBrace,

    [Description(")")] CloseParen,

    [Description("~")] Complement,

    [Description("")] Constant,

    [Description("--")] Decrement,

    [Description("/")] DivSign,

    [Description("==")] EqualTo,

    [Description(">=")] GreaterOrEqual,

    [Description(">")] GreaterThan,

    [Description("")] Identifier,

    [Description("++")] Increment,

    [Description("int")] IntKw,

    [Description("<=")] LessOrEqual,

    [Description("<")] LessThan,

    [Description("&&")] LogicalAnd,

    [Description("!")] LogicalNot,

    [Description("||")] LogicalOr,

    [Description("-")] MinusSign,

    [Description("%")] ModSign,

    [Description("*")] MulSign,

    [Description("!=")] NotEqualTo,

    [Description("{")] OpenBrace,

    [Description("(")] OpenParen,

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