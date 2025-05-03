namespace Wacc.Tests;

[TestClass]
public class RuntimeStateTests
{
    [TestMethod]
    [DataRow(true, false, false, false, true, false, false, false)]
    [DataRow(false, true, false, false, true, true, false, false)]
    [DataRow(false, false, true, false, true, true, true, false)]
    [DataRow(false, false, false, true, true, true, true, true)]
    [DataRow(false, false, false, false, true, true, true, true)]
    public void StageFlags(bool throughLex, bool throughParse, bool throughCodeGen, bool throughCodeEmit, bool doLex, bool doParse, bool doCodeGen, bool doCodeEmit)
    {
        var rts = new RuntimeState()
        {
            InputFile = "",
            OnlyThroughLexer = throughLex,
            OnlyThroughParser = throughParse,
            OnlyThroughCodeGen = throughCodeGen,
            OnlyThroughCodeEmit = throughCodeEmit,
        };

        Assert.AreEqual(doLex, rts.DoLexer);
        Assert.AreEqual(doParse, rts.DoParser);
        Assert.AreEqual(doCodeGen, rts.DoCodeGen);
        Assert.AreEqual(doCodeEmit, rts.DoCodeEmission);
    }
}