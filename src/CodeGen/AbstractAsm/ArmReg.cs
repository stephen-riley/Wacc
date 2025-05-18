namespace Wacc.CodeGen.AbstractAsm;

public enum ArmReg
{
    // system registers
    SP = 1000,
    FP,
    LR,
    PSTATE,

    // calling convention registers
    W0 = 0,
    W1,
    W2,
    W3,
    W4,
    W5,
    W6,
    W7,
    W8,

    // scratch registers 9-15
    W9,
    W10,
    W11,
    W12,
    W13,
    W14,
    W15,

    // special definitions
    RETVAL = W0,
    SCRATCH1 = W9,
    SCRATCH2 = W10,
    SCRATCH3 = W11,
    LAST_REGISTER = W15,
}