using Wacc.Lex;
using Wacc.Parse;

namespace Wacc.Tests;

[TestClass]
public class ParserTests
{
    private static readonly RuntimeState DummyRts = new() { InputFile = "", Verbose = true };
    private const string fixturesPath = "../../../../fixtures";

    [TestMethod]
    [DataRow("return_2.c")]
    [DataRow("comments.c")]
    [DataRow("listing2-1.c")]
    [DataRow("multi_digit.c")]
    [DataRow("multiline_comments.c")]
    [DataRow("return_2.c")]
    [DataRow("two_plus_three.c")]
    [DataRow("complex_binary_expr.c")]
    [DataRow("ignore_preprocessor.c")]
    [DataRow("shifting.c")]
    [DataRow("not_sum.c")]
    public void SimpleParse(string filename)
    {
        var text = File.ReadAllText($"{fixturesPath}/valid/{filename}");
        var lexer = new Lexer(DummyRts);
        _ = lexer.Lex(text);
        var parser = new Parser(DummyRts);
        parser.Parse();
    }
}