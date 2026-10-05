using System.Text.RegularExpressions;
using Wacc.Exceptions;
using Wacc.Tokens;
using static Wacc.Tokens.TokenType;

namespace Wacc.Lex;

public class Lexer(RuntimeState opts)
{
    public record MultilineComment(int Index, int LineIndex, string Content);

    public RuntimeState Options = opts;

    private readonly HashSet<TokenType> IgnoredTokens = [WHITESPACE, COMMENT_SINGLE_LINE, COMMENT_MULTILINE, PREPROCESSOR_DIRECTIVE];

    public OrderedDictionary<TokenType, Regex> Patterns { get; set; } = new()
    {
        // DO NOT CHANGE ORDER
        { PREPROCESSOR_DIRECTIVE, new Regex(@"\G#.*$", RegexOptions.Multiline) },
        { COMMENT_SINGLE_LINE, new Regex(@"\G//.*$", RegexOptions.Multiline) },
        { COMMENT_MULTILINE, new Regex(@"\G/\*.*?\*/", RegexOptions.Singleline) },
        // This next is multiline because COMMENT_MULTILINE will have picked
        //  up a single line /* ... */.  If it didn't, then we want to grab
        //  the rest of line.
        { COMMENT_MULTILINE_OPEN, new Regex(@"\G/\*.*?$", RegexOptions.Multiline) },
        // { COMMENT_MULTILINE_CLOSE, new Regex(@"\G.*?\*/", RegexOptions.Singleline) },
        { COMMENT_MULTILINE_CLOSE, new Regex(@"\G(?:(?!/\*).)*?\*/") },
        { WHITESPACE, new Regex(@"\G\s+") },
        { IntKw, new Regex(@"\Gint\b") },
        { VoidKw, new Regex(@"\Gvoid\b") },
        { ReturnKw, new Regex(@"\Greturn\b") },
        { IfKw, new Regex(@"\Gif\b") },
        { ElseKw, new Regex(@"\Gelse\b") },
        { BreakKw, new Regex(@"\Gbreak\b") },
        { ContinueKw, new Regex(@"\Gcontinue\b") },
        { DefaultKw, new Regex(@"\Gdefault\b") },
        { DoKw, new Regex(@"\Gdo\b") },
        { ForKw, new Regex(@"\Gfor\b") },
        { WhileKw, new Regex(@"\Gwhile\b") },
        { GotoKw, new Regex(@"\Ggoto\b") },
        { SwitchKw, new Regex(@"\Gswitch\b") },
        { CaseKw, new Regex(@"\Gcase\b") },
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
        { Comma, new Regex(@"\G,") },
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
        if (text.Contains('\n'))
        {
            throw new InvalidOperationException($"Do not pass multiline strings to {nameof(Lexer)}:{nameof(Lex)}(string Text)");
        }
        else
        {
            return Lex([text], includeIgnored);
        }
    }

    public List<Token> Lex(string[] text, bool includeIgnored = false)
    {
        if (!text.Any())
        {
            Options.TokenStream = [];
            return [];
        }

        var tokens = new List<Token>();

        var index = 0;
        var lineIndex = 0;
        var line = text[lineIndex];

        MultilineComment? mlc = null;


    OUTER_LOOP:
        while (lineIndex < text.Length)
        {
            if (index >= line.Length)
            {
                lineIndex++;
                if (lineIndex == text.Length)
                {
                    break;
                }
                line = text[lineIndex];
                index = 0;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                index = 0;
                line = text[++lineIndex];
                goto OUTER_LOOP;
            }

            if (mlc is not null)
            {
                if (!Patterns[COMMENT_MULTILINE].IsMatch(line, startat: index) && !Patterns[COMMENT_MULTILINE_CLOSE].IsMatch(line, startat: index))
                {
                    mlc = mlc with { Content = mlc.Content + line + '\n' };
                    index = 0;
                    line = text[++lineIndex];
                    goto OUTER_LOOP;
                }
            }

            foreach (var (tok, re) in Patterns)
            {
                var match = re.Match(line, startat: index);
                if (match.Success)
                {
                    if (includeIgnored || !IgnoredTokens.Contains(tok))
                    {
                        var s = match.Value;
                        Token t = null!;

                        if (string.IsNullOrWhiteSpace(s))
                        {
                            s = $"'{s.Replace('\n', '␤')}'";
                        }

                        if (tok == COMMENT_MULTILINE_OPEN)
                        {
                            mlc = new MultilineComment(index, lineIndex, line[index..] + "\n");
                            index = 0;
                            line = text[++lineIndex];
                            goto OUTER_LOOP;
                        }
                        else if (tok == COMMENT_MULTILINE_CLOSE)
                        {
                            if (mlc is null)
                            {
                                throw new LexerError($"Unexpected {tok} at line {lineIndex + 1}, column {index + 1}");
                            }

                            t = new Token(COMMENT_MULTILINE, mlc.Index + 1, mlc.LineIndex + 1, mlc.Content + s, 0);
                            mlc = null;
                        }
                        else if (mlc is not null)
                        {
                            mlc = mlc with { Content = mlc.Content + line + '\n' };
                            index += 0;
                            line = text[++lineIndex];
                            goto OUTER_LOOP;
                        }
                        else
                        {
                            _ = int.TryParse(s, out var i);
                            t = new Token(tok, lineIndex + 1, index + 1, s, i);
                        }

                        if (includeIgnored || !IgnoredTokens.Contains(t.TokenType))
                        {
                            tokens.Add(t);
                        }
                    }
                    index += match.Value.Length;
                    goto OUTER_LOOP;
                }
            }

            if (mlc is null)
            {
                throw new LexerError($"Cannot tokenize '{line[index..].Replace('\n', '␤')}'");
            }
            else
            {
                mlc = mlc with { Content = mlc.Content + line[index..] };
                index = line.Length;
            }
        }

        if (mlc is not null)
        {
            throw new LexerError($"Multiline comment starting at {mlc.LineIndex + 1}:{mlc.Index + 1} is unterminated.");
        }

        Options.TokenStream = tokens;
        return tokens;
    }
}