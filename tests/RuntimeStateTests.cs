namespace Wacc.Tests;

[TestClass]
public class RuntimeStateTests
{
    [TestMethod]
    // through lex
    [DataRow(true, false, false, false, false, false,
                true, false, false, false, false, false)]
    // through parse
    [DataRow(false, true, false, false, false, false,
                true, true, false, false, false, false)]
    // through validate
    [DataRow(false, false, true, false, false, false,
                true, true, true, false, false, false)]
    // through Tacky
    [DataRow(false, false, false, true, false, false,
                true, true, true, true, false, false)]
    // through code gen
    [DataRow(false, false, false, false, true, false,
                true, true, true, true, true, false)]
    // through emit
    [DataRow(false, false, false, false, false, true,
                true, true, true, true, true, true)]
    public void StageFlags(bool throughLex, bool throughParse, bool throughValidate, bool throughTacky, bool throughCodeGen, bool throughCodeEmit,
                            bool doLex, bool doParse, bool doValidate, bool doTacky, bool doCodeGen, bool doCodeEmit)
    {
        var rts = new RuntimeState()
        {
            InputFile = "",
            OnlyThroughLexer = throughLex,
            OnlyThroughParser = throughParse,
            OnlyThroughValidate = throughValidate,
            OnlyThroughTacky = throughTacky,
            OnlyThroughCodeGen = throughCodeGen,
            OnlyThroughCodeEmit = throughCodeEmit,
        };

        Assert.AreEqual(doLex, rts.DoLexer);
        Assert.AreEqual(doParse, rts.DoParser);
        Assert.AreEqual(doValidate, rts.DoValidate);
        Assert.AreEqual(doTacky, rts.DoTacky);
        Assert.AreEqual(doCodeGen, rts.DoCodeGen);
        Assert.AreEqual(doCodeEmit, rts.DoCodeEmission);
    }

    [TestMethod]
    [DataRow("/tmp/in.c", null, "/tmp/in")]
    [DataRow(null, "/tmp/out.S", "/tmp/out")]
    [DataRow("/tmp/in.c", "/tmp/out.S", "/tmp/out")]
    [DataRow("/tmp/fun.in.c", "/tmp/fun.out.S", "/tmp/fun.out")]
    public void BaseFilename(string? inputFile, string? outputFile, string expected)
    {
#nullable disable
        var rts = new RuntimeState() { InputFile = inputFile, OutputFile = outputFile };
#nullable restore
        Assert.AreEqual(expected, rts.BaseFilename);
    }
}