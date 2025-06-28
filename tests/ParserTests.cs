using Wacc.Ast;
using Wacc.Lex;
using Wacc.Parse;
using Wacc.Tokens;

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

    [TestMethod]
    public void ParseDeclarationNoExpression()
    {
        var lexer = new Lexer(new RuntimeState() { InputFile = "" });
        var code = "int a;";
        var tokens = lexer.Lex(code);
        var tokenStream = new Queue<Token>(tokens);

        Assert.IsTrue(Declaration.CanParse(tokenStream));
        var decl = Declaration.Parse(tokenStream);
        Assert.AreEqual("a", decl.Identifier.Name);
        Assert.IsNull(decl.Expr);
    }

    [TestMethod]
    public void ParseDeclarationWithExpression()
    {
        var lexer = new Lexer(new RuntimeState() { InputFile = "" });
        var code = "int a = 2 * 3;";
        var tokens = lexer.Lex(code);
        var tokenStream = new Queue<Token>(tokens);

        Assert.IsTrue(Declaration.CanParse(tokenStream));
        var decl = Declaration.Parse(tokenStream);
        Assert.AreEqual("a", decl.Identifier.Name);
        Assert.IsInstanceOfType<BinaryOp>(decl.Expr);
    }
}