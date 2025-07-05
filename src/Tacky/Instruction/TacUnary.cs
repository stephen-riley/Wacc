using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Tacky.Instruction;

public record TacUnary(TokenType Op, TacVal Src, TacVar Dst) : ITackyInstr
{
    public string OpName => Op switch
    {
        Minus => "Negate",
        Complement => "Complement",
        LogicalNot => "Not",
        _ => throw new InvalidOperationException($"no unary operator '{Op}'")
    };

    public TacVar GetDst() => Dst;

    public override string ToString() => $"Unary({OpName}, {Src}, {Dst})";
}