using Wacc.Exceptions;
using Wacc.Lex;

namespace Wacc.Tests;

[TestClass]
public class LexerTests
{
    private static readonly RuntimeState DummyRts = new() { InputFile = "" };
    private const string fixturesPath = "../../../../fixtures";

    [TestMethod]
    [DataRow("int", "[IntKw:int (0)]")]
    [DataRow("12", "[Constant:12 (0)]")]
    [DataRow("return 3;", "[ReturnKw:return (0)], [Constant:3 (7)], [Semicolon:; (8)]")]
    public void LexerHappyPath(string text, string expected)
    {
        var lexer = new Lexer(DummyRts);
        var toks = lexer.Lex(text);
        var toksString = toks.ToTokenString();
        Assert.AreEqual(expected, toksString);
    }

    [TestMethod]
    [DataRow("( )", "[OpenParen:( (0)], [WHITESPACE:' ' (1)], [CloseParen:) (2)]")]
    [DataRow("// comment", "[COMMENT_SINGLE_LINE:// comment (0)]")]
    public void LexerHappyPathIncludeIgnored(string text, string expected)
    {
        var lexer = new Lexer(DummyRts);
        var toks = lexer.Lex(text, includeIgnored: true);
        var toksString = toks.ToTokenString();
        Assert.AreEqual(expected, toksString);
    }

    [TestMethod]
    [DataRow("123abc", "Cannot tokenize '123abc'")]
    [DataRow("int main(void) { return 123abc; }", "Cannot tokenize '123abc; }'")]
    public void LexerExecptions(string text, string expectedMessage)
    {
        var lexer = new Lexer(DummyRts);
        var ex = Assert.ThrowsException<LexerError>(() =>
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
        var text = File.ReadAllText($"{fixturesPath}/{filename}");
        var lexer = new Lexer(DummyRts);
        lexer.Lex(text);
        // Test passes if no exception
    }
}
