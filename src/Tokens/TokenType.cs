using System.ComponentModel;
namespace Wacc.Tokens;

public enum TokenType
{
    [Description("=")] Assign,

    [Description("*")] Asterisk,

    [Description("&")] BitwiseAnd,

    [Description("<<")] BitwiseLeft,

    [Description("|")] BitwiseOr,

    [Description(">>")] BitwiseRight,

    [Description("^")] BitwiseXor,

    [Description("}")] CloseBrace,

    [Description(")")] CloseParen,

    [Description(":")] Colon,

    [Description("~")] Complement,

    [Description("&=")] CompoundBitwiseAnd,

    [Description("<<=")] CompoundBitwiseLeft,

    [Description("|=")] CompoundBitwiseOr,

    [Description(">>=")] CompoundBitwiseRight,

    [Description("^=")] CompoundBitwiseXor,

    [Description("/=")] CompoundDiv,

    [Description("-=")] CompoundMinus,

    [Description("%=")] CompoundMod,

    [Description("*=")] CompoundMul,

    [Description("+=")] CompoundPlus,

    [Description("")] Constant,

    [Description("--")] Decrement,

    [Description("/")] Div,

    [Description("break")] BreakKw,

    [Description("continue")] ContinueKw,

    [Description("do")] DoKw,

    [Description("else")] ElseKw,

    [Description("==")] EqualTo,

    [Description("for")] ForKw,

    [Description("goto")] GotoKw,

    [Description(">=")] GreaterOrEqual,

    [Description(">")] GreaterThan,

    [Description("")] Identifier,

    [Description("if")] IfKw,

    [Description("++")] Increment,

    [Description("int")] IntKw,

    [Description("while")] WhileKw,

    [Description("<=")] LessOrEqual,

    [Description("<")] LessThan,

    [Description("&&")] LogicalAnd,

    [Description("!")] LogicalNot,

    [Description("||")] LogicalOr,

    [Description("-")] Minus,

    [Description("%")] Mod,

    [Description("!=")] NotEqualTo,

    [Description("{")] OpenBrace,

    [Description("(")] OpenParen,

    [Description("+")] Plus,

    [Description("?")] Question,

    [Description("return")] ReturnKw,

    [Description(";")] Semicolon,

    [Description("void")] VoidKw,

    [Description("<WHITESPACE>")] WHITESPACE,

    [Description("<// COMMENT>")] COMMENT_SINGLE_LINE,

    [Description("</* COMMENT */>")] COMMENT_MULTI_LINE,

    [Description("<#DIRECTIVE>")] PREPROCESSOR_DIRECTIVE,

    [Description("<EOF>")] EOF
}