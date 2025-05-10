using Wacc.CodeGen;
using Wacc.CodeGen.AbstractAsm;
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
    [DataRow("tmp.0", Register.W9)]
    [DataRow("tmp.6", Register.W15)]
    public void TmpToRegisterDefaultOffset(string tmp, Register r)
    => Assert.AreEqual(r, CodeGenerator.AssignRegisterForTmp(tmp));

    [TestMethod]
    [DataRow("tmp.23")]
    [DataRow("tmp.-3")]
    public void InvalidTmpToRegister(string tmp)
        => Assert.ThrowsException<CodeGenError>(() => CodeGenerator.AssignRegisterForTmp(tmp));
}