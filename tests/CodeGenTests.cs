using Wacc.CodeGen;
using Wacc.CodeGen.AbstractAsm;
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.Exceptions;

namespace Wacc.Tests;

[TestClass]
public class CodeGenTests
{
    [TestMethod]
    [DataRow("tmp.0", Register.W0)]
    [DataRow("tmp.15", Register.W15)]
    public void TmpToRegisterZeroOffset(string tmp, Register r)
        => Assert.AreEqual(r, CodeGenerator.AssignRegisterForTmp(tmp, baseRegister: 0));

    [TestMethod]
    [DataRow("tmp.0", Register.W10)]
    [DataRow("tmp.5", Register.W15)]
    public void TmpToRegisterDefaultOffset(string tmp, Register r)
    => Assert.AreEqual(r, CodeGenerator.AssignRegisterForTmp(tmp));

    [TestMethod]
    [DataRow("tmp.23")]
    [DataRow("tmp.-3")]
    public void InvalidTmpToRegister(string tmp)
        => Assert.ThrowsException<CodeGenError>(() => CodeGenerator.AssignRegisterForTmp(tmp));

    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(1, 16)]
    [DataRow(3, 16)]
    [DataRow(5, 32)]
    public void StackSpaceIsMultipleOf16(int vars, int expectedStackSpace)
    {
        var f = new AsmFunction("test");
        int offset = 0;
        for (var i = 0; i < vars; i++)
        {
            f.StackOffsets[(i + 'a').ToString()] = offset;
            offset -= 4;
        }

        Assert.AreEqual(expectedStackSpace, f.LocalsSize);
    }
}