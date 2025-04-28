using System.Text.RegularExpressions;
using Wacc.Exceptions;
using static Wacc.TokenType;

namespace Wacc;

public class Lexer(RuntimeState opts)
{
    public RuntimeState Options = opts;

    public OrderedDictionary<TokenType, Regex> Patterns { get; set; } = new()
    {
        { WHITESPACE, new Regex(@"\G\s+") },
        { IntKw, new Regex(@"\Gint\b") },
        { VoidKw, new Regex(@"\Gvoid\b") },
        { ReturnKw, new Regex(@"\Greturn\b") },
        { Identifier, new Regex(@"\G[a-zA-Z_]\w*\b") },
        { Constant, new Regex(@"\G[0-9]+\b") },
        { OpenParen, new Regex(@"\G\(") },
        { CloseParen, new Regex(@"\G\)") },
        { OpenBrace, new Regex(@"\G{") },
        { CloseBrace, new Regex(@"\G}") },
        { Semicolon, new Regex(@"\G;") },
    };

    public bool Execute()
    {
        Options.TokenStream = Lex(Options.Text);

        foreach (var t in Options.TokenStream)
        {
            Console.WriteLine(t);
        }

        return true;
    }

    public List<Token> Lex(string text)
    {
        var tokens = new List<Token>();

        var index = 0;

    OUTER_LOOP:
        while (index < text.Length)
        {
            foreach (var (tok, re) in Patterns)
            {
                var match = re.Match(text, startat: index);
                if (match.Success)
                {
                    if (tok != WHITESPACE)
                    {
                        var s = match.Value;
                        int.TryParse(s, out var i);
                        var t = new Token(tok, index, s, i);
                        tokens.Add(t);
                    }
                    index += match.Value.Length;
                    goto OUTER_LOOP;
                }
            }

            throw new LexerException($"Cannot tokenize '{text[index..].Replace('\n', ' ')}'");
        }

        return tokens;
    }
}