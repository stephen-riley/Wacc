using CommandLine;
using Wacc.Ast;
using Wacc.Tokens;

namespace Wacc;

public class RuntimeState
{
    [Option('l', "lex", HelpText = "Run the lexer, but stop before parsing")]
    public bool OnlyThroughLexer { get; set; } = false;

    [Option('p', "parse", HelpText = "Run the lexer and parser, but stop before assembly generation")]
    public bool OnlyThroughParser { get; set; } = false;

    [Option('c', "codegen", HelpText = "Perform lexing, parsing, and assembly generation, but stop before code emission")]
    public bool OnlyThroughCodeGen { get; set; } = false;

    [Option('v', "verbose", HelpText = "Verbose output on STDERR")]
    public bool Verbose { get; set; }

    public bool DoLexer { get; } = true;
    public bool DoParser => !OnlyThroughParser;
    public bool DoCodeGen => !OnlyThroughLexer && !OnlyThroughParser;
    public bool DoCodeEmission => !OnlyThroughLexer && !OnlyThroughParser && !OnlyThroughCodeGen;

    [Value(0, MetaName = "input file", HelpText = ".c file to compile", Required = true)]
    public required string InputFile { get; set; }

    public string Text { get; set; } = "";

    public List<Token> TokenStream { get; set; } = [];

    public IAstNode Ast { get; set; } = null!;
}