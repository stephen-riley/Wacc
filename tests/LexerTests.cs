using Wacc.Exceptions;

namespace Wacc.Tests;

[TestClass]
public class LexerTests
{
    private static readonly RuntimeState DummyRts = new() { InputFile = "" };

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
    [DataRow("123abc", "Cannot tokenize '123abc'")]
    public void LexerExections(string text, string expectedMessage)
    {
        var lexer = new Lexer(DummyRts);
        var ex = Assert.ThrowsException<LexerException>(() =>
        {
            var toks = lexer.Lex(text);
        });
        Assert.AreEqual(expectedMessage, ex.Message);
    }
}
