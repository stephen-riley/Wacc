using Wacc.Parse;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Tests;

[TestClass]
public class ParserExtensionTests
{
    private static readonly RuntimeState VoidRts = new() { InputFile = "", Verbose = true };
    private const string fixturesPath = "../../../../fixtures";

    [TestMethod]
    public void PeekForSequenceHappyPath()
    {
        IEnumerable<TokenType> sequence = [IntKw, Identifier, Semicolon];
        Queue<Token> tokenStream = new(new List<TokenType>([IntKw, Identifier, Semicolon, VoidKw, OpenParen, CloseParen]).Select((tt, i) => new Token(tt, 1, i, "dummy", -1)));
        Assert.IsTrue(tokenStream.PeekForSequence(sequence));
    }

    [TestMethod]
    public void PeekForSequenceStreamTooShort()
    {
        IEnumerable<TokenType> sequence = [IntKw, Identifier, Semicolon];
        Queue<Token> tokenStream = new(new List<TokenType>([IntKw, Identifier]).Select((tt, i) => new Token(tt, 1, i, "dummy", -1)));
        Assert.IsFalse(tokenStream.PeekForSequence(sequence));
    }

    [TestMethod]
    public void PeekForSequenceNotEqual()
    {
        IEnumerable<TokenType> sequence = [IntKw, Identifier, Semicolon];
        Queue<Token> tokenStream = new(new List<TokenType>([IntKw, Identifier, OpenParen]).Select((tt, i) => new Token(tt, 1, i, "dummy", -1)));
        Assert.IsFalse(tokenStream.PeekForSequence(sequence));
    }
}