using Wacc.Exceptions;
using Wacc.Lex;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Tests;

[TestClass]
public class LexerTests
{
    private static readonly RuntimeState VoidRts = new() { InputFile = "" };
    private const string fixturesPath = "../../../../fixtures";

    [TestMethod]
    [DataRow("int", "[IntKw:int (1:1)]")]
    [DataRow("12", "[Constant:12 (1:1)]")]
    [DataRow("return 3;", "[ReturnKw:return (1:1)], [Constant:3 (1:8)], [Semicolon:; (1:9)]")]
    [DataRow("+ - * / %", "[Plus:+ (1:1)], [Minus:- (1:3)], [Asterisk:* (1:5)], [Div:/ (1:7)], [Mod:% (1:9)]")]
    public void LexerHappyPath(string text, string expected)
    {
        var lexer = new Lexer(VoidRts);
        var toks = lexer.Lex(text);
        var toksString = toks.ToTokenString();
        Assert.AreEqual(expected, toksString);
    }

    [TestMethod]
    [DataRow("( )", "[OpenParen:( (1:1)], [WHITESPACE:' ' (1:2)], [CloseParen:) (1:3)]")]
    [DataRow("// comment", "[COMMENT_SINGLE_LINE:// comment (1:1)]")]
    public void LexerHappyPathIncludeIgnored(string text, string expected)
    {
        var lexer = new Lexer(VoidRts);
        var toks = lexer.Lex(text, includeIgnored: true);
        var toksString = toks.ToTokenString();
        Assert.AreEqual(expected, toksString);
    }

    [TestMethod]
    [DataRow("123abc", "Cannot tokenize '123abc'")]
    [DataRow("int main(void) { return 123abc; }", "Cannot tokenize '123abc; }'")]
    public void LexerExecptions(string text, string expectedMessage)
    {
        var lexer = new Lexer(VoidRts);
        var ex = Assert.Throws<LexerError>(() =>
        {
            var toks = lexer.Lex(text);
        });
        Assert.AreEqual(expectedMessage, ex.Message);
    }

    [TestMethod]
    [DataRow("comments.c")]
    [DataRow("multiline_comments.c")]
    public void LexComments(string filename)
    {
        var text = File.ReadAllLines($"{fixturesPath}/valid/{filename}");
        var lexer = new Lexer(VoidRts);
        lexer.Lex(text);
        // Test passes if no exception
    }

    [TestMethod]
    [DataRow("at_sign.c")]
    [DataRow("invalid_identifier_2.c")]
    public void LexExpectFailure(string filename)
    {
        var text = File.ReadAllLines($"{fixturesPath}/invalid/{filename}");
        var lexer = new Lexer(VoidRts);
        Assert.Throws<LexerError>(() =>
        {
            lexer.Lex(text);
        });
    }

    [TestMethod]
    public void LexMultiLineInput()
    {
        string[] text = [.. """
        int main(void) {
            int a = 1;
        }
        """.Split('\n')];
        var expectedToks = new List<Token>()
        {
            new(IntKw, 1, 1, "int", 0),
            new(Identifier, 1, 5, "main", 0),
            new(OpenParen, 1, 9, "(", 0),
            new(VoidKw, 1, 10, "void", 0),
            new(CloseParen, 1, 14, ")", 0),
            new(OpenBrace, 1, 16, "{", 0),
            new(IntKw, 2, 5, "int", 0),
            new(Identifier, 2, 9, "a", 0),
            new(Assign, 2, 11, "=", 0),
            new(Constant, 2, 13, "1", 1),
            new(Semicolon, 2, 14, ";", 0),
            new(CloseBrace, 3, 1, "}", 0)
        };

        var lexer = new Lexer(VoidRts);
        var tokenStream = lexer.Lex(text);

        Assert.AreEqual(tokenStream.Count, expectedToks.Count);
        Assert.IsTrue(tokenStream.SequenceEqual(expectedToks));
    }

    [TestMethod]
    [DataRow("""
        /* hello there */
        """,
        true)]
    [DataRow("""
        /* 
            hello there 
        */
        """,
        true)]
    [DataRow("""
        /* 
            hello there 
        """,
        false)]
    [DataRow("""
        int a;
        */
        """,
        false)]
    public void LexMultilineComments(string text, bool shouldSucceed)
    {
        var lexer = new Lexer(VoidRts);

        try
        {
            lexer.Lex([.. text.Split('\n')]);
        }
        catch
        {
            if (shouldSucceed)
            {
                throw;
            }
        }
    }
}
