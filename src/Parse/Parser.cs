using Wacc.Tokens;

namespace Wacc.Parse;

public class Parser(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute() => Parse();

    public bool Parse()
    {
        var toks = new Queue<Token>(Options.TokenStream);
        var program = Ast.Program.Parse(toks);
        return true;
    }
}