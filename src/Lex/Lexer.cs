using System.Text.RegularExpressions;
using Wacc.Exceptions;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Lex;

public class Lexer(RuntimeState opts)
{
    public RuntimeState Options = opts;

    private HashSet<TokenType> IgnoredTokens = [WHITESPACE, COMMENT_SINGLE_LINE, COMMENT_MULTI_LINE];

    public OrderedDictionary<TokenType, Regex> Patterns { get; set; } = new()
    {
        { COMMENT_SINGLE_LINE, new Regex(@"\G//.*$",RegexOptions.Multiline)},
        { COMMENT_MULTI_LINE, new Regex(@"\G/\*.*?\*/",RegexOptions.Singleline)},
        { WHITESPACE, new Regex(@"\G\s+") },
        { IntKw, new Regex(@"\Gint\b") },
        { VoidKw, new Regex(@"\Gvoid\b") },
        { ReturnKw, new Regex(@"\Greturn\b") },
        { Identifier, new Regex(@"\G[a-zA-Z_]\w*\b") },
        { Decrement, new Regex(@"\G--")},
        { Constant, new Regex(@"\G-?[0-9]+\b") },
        { OpenParen, new Regex(@"\G\(") },
        { CloseParen, new Regex(@"\G\)") },
        { OpenBrace, new Regex(@"\G{") },
        { CloseBrace, new Regex(@"\G}") },
        { Semicolon, new Regex(@"\G;") },
        { Complement, new Regex(@"\G~")},
        { Negate, new Regex(@"\G-")},
        { EOF, new Regex(@"\G$", RegexOptions.Multiline) },
    };

    public bool Execute()
    {
        Options.TokenStream = Lex(Options.Text);

        if (Options.Verbose || Options.OnlyThroughLexer)
        {
            if (Options.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("TOKENS:");
            }

            var stream = Options.Verbose ? Console.Error : Console.Out;

            foreach (var t in Options.TokenStream)
            {
                stream.WriteLine(t);
            }
        }

        return true;
    }

    public List<Token> Lex(string text, bool includeIgnored = false)
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
                    if (includeIgnored || !IgnoredTokens.Contains(tok))
                    {
                        var s = match.Value;
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            s = $"'{s.Replace('\n', '␤')}'";
                        }
                        int.TryParse(s, out var i);
                        var t = new Token(tok, index, s, i);
                        tokens.Add(t);
                    }
                    index += match.Value.Length;
                    goto OUTER_LOOP;
                }
            }

            throw new LexerError($"Cannot tokenize '{text[index..].Replace('\n', '␤')}'");
        }

        Options.TokenStream = tokens;
        return tokens;
    }
}