using Wacc.Lex;
using Wacc.Tokens;

namespace Wacc.Tests;

[TestClass]
public class ParserTests
{
    private static readonly RuntimeState DummyRts = new() { InputFile = "" };
    private const string fixturesPath = "../../../../fixtures";

    [TestMethod]
    public void SimpleParse()
    {
        var text = File.ReadAllText($"{fixturesPath}/return_2.c");
        var lexer = new Lexer(DummyRts);
        var tokens = lexer.Lex(text);
        var program = Wacc.Ast.Program.Parse(new Queue<Token>(tokens));
    }
}