using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Wacc.Exceptions;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Lex;

public class Lexer(RuntimeState opts)
{
    public RuntimeState Options = opts;

    private readonly HashSet<TokenType> IgnoredTokens = [WHITESPACE, COMMENT_SINGLE_LINE, COMMENT_MULTI_LINE, PREPROCESSOR_DIRECTIVE];

    public OrderedDictionary<TokenType, Regex> Patterns { get; set; } = new()
    {
        // DO NOT CHANGE ORDER
        { PREPROCESSOR_DIRECTIVE, new Regex(@"\G#.*$", RegexOptions.Multiline) },
        { COMMENT_SINGLE_LINE, new Regex(@"\G//.*$", RegexOptions.Multiline) },
        { COMMENT_MULTI_LINE, new Regex(@"\G/\*.*?\*/", RegexOptions.Singleline) },
        { WHITESPACE, new Regex(@"\G\s+") },
        { IntKw, new Regex(@"\Gint\b") },
        { VoidKw, new Regex(@"\Gvoid\b") },
        { ReturnKw, new Regex(@"\Greturn\b") },
        { IfKw, new Regex(@"\Gif\b") },
        { ElseKw, new Regex(@"\Gelse\b") },
        { GotoKw, new Regex(@"\Ggoto\b") },
        { Identifier, new Regex(@"\G[a-zA-Z_]\w*\b") },
        { Constant, new Regex(@"\G[0-9]+\b") },
        { Colon, new Regex(@"\G:") },
        { Question, new Regex(@"\G\?") },
        { OpenParen, new Regex(@"\G\(") },
        { CloseParen, new Regex(@"\G\)") },
        { OpenBrace, new Regex(@"\G{") },
        { CloseBrace, new Regex(@"\G}") },
        { Semicolon, new Regex(@"\G;") },
        { Complement, new Regex(@"\G~") },
        { CompoundPlus, new Regex(@"\G\+=") },
        { CompoundMinus, new Regex(@"\G\-=") },
        { CompoundMul, new Regex(@"\G\*=") },
        { CompoundDiv, new Regex(@"\G\/=") },
        { CompoundMod, new Regex(@"\G\%=") },
        { CompoundBitwiseAnd, new Regex(@"\G\&=") },
        { CompoundBitwiseOr, new Regex(@"\G\|=") },
        { CompoundBitwiseXor, new Regex(@"\G\^=") },
        { CompoundBitwiseLeft, new Regex(@"\G<<=") },
        { CompoundBitwiseRight, new Regex(@"\G>>=") },
        { Increment, new Regex(@"\G\+\+") },
        { Decrement, new Regex(@"\G\-\-") },
        { Plus, new Regex(@"\G\+") },
        { Minus, new Regex(@"\G-") },
        { Asterisk, new Regex(@"\G\*") },
        { Div, new Regex(@"\G/") },
        { Mod, new Regex(@"\G%") },
        { LogicalAnd, new Regex(@"\G&&") },
        { BitwiseAnd, new Regex(@"\G\&") },
        { BitwiseLeft, new Regex(@"\G<<") },
        { LogicalOr, new Regex(@"\G\|\|") },
        { BitwiseOr, new Regex(@"\G\|") },
        { BitwiseRight, new Regex(@"\G>>") },
        { BitwiseXor, new Regex(@"\G\^") },
        { EqualTo, new Regex(@"\G==") },
        { NotEqualTo, new Regex(@"\G!=") },
        { LogicalNot, new Regex(@"\G!") },
        { LessOrEqual, new Regex(@"\G<=") },
        { LessThan, new Regex(@"\G<") },
        { GreaterOrEqual, new Regex(@"\G>=") },
        { GreaterThan, new Regex(@"\G>") },
        { Assign, new Regex(@"\G\G=") },
        { EOF, new Regex(@"\G$", RegexOptions.Multiline) },
    };

    public bool Execute()
    {
        Options.TokenStream = Lex(Options.Text);

        if (!Options.Silent)
        {
            if (Options.Verbose || Options.OnlyThroughLexer)
            {
                if (Options.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("TOKENS:");
                    Console.Error.WriteLine("========");
                }

                var stream = Options.Verbose ? Console.Error : Console.Out;

                foreach (var t in Options.TokenStream)
                {
                    stream.WriteLine(t);
                }
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
                        _ = int.TryParse(s, out var i);
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