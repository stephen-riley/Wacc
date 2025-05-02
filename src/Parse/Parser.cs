using Wacc.Tokens;

namespace Wacc.Parse;

public class Parser(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public bool Execute() => Parse();

    public bool Parse()
    {
        var toks = new Queue<Token>(Options.TokenStream);
        toks.Enqueue(new Token(TokenType.EOF, Options.Text.Length + 1, "", 0));
        var program = Ast.Program.Parse(toks);
        return true;
    }
}