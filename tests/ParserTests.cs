using Wacc.Lex;
using Wacc.Parse;
using Wacc.Tokens;

namespace Wacc.Tests;

[TestClass]
public class ParserTests
{
    private static readonly RuntimeState DummyRts = new() { InputFile = "" };
    private const string fixturesPath = "../../../../fixtures";

    [TestMethod]
    [DataRow("return_2.c")]
    public void SimpleParse(string filename)
    {
        var text = File.ReadAllText($"{fixturesPath}/valid/{filename}");
        var lexer = new Lexer(DummyRts);
        var tokens = lexer.Lex(text);
        var parser = new Parser(DummyRts);
        parser.Parse();
    }
}