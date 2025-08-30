using Wacc.Ast;
using Wacc.Exceptions;
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
        var program = CompUnit.Parse(toks);
        Options.Ast = program ?? throw new ParseError("Parsing did not return a Program AST node");

        if (!Options.Silent)
        {
            if (Options.Verbose || Options.OnlyThroughParser)
            {
                if (Options.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("AST:");
                    Console.Error.WriteLine("=====");
                }

                var stream = Options.Verbose ? Console.Error : Console.Out;
                stream.WriteLine(program.ToPrettyString());
            }
        }

        return true;
    }
}