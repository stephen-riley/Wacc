using System.Text.RegularExpressions;
using CommandLine;
using Wacc.AbstractAsm;
using Wacc.Ast;
using Wacc.Tacky.Instruction;
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

    [Option('e', "emit", HelpText = "Perform lexing, parsing, assembly generation, and code emission")]
    public bool OnlyThroughCodeEmit { get; set; } = false;

    [Option('v', "verbose", HelpText = "Verbose output on STDERR")]
    public bool Verbose { get; set; }

    [Option('S', HelpText = "Stop after assembly")]
    public bool StopAfterAssembly { get; set; }

    [Option('o', "output", HelpText = "Output exe filename")]
    public string? OutputFile { get; set; }

    public bool DoLexer { get; } = true;
    public bool DoParser => !OnlyThroughLexer;
    public bool DoCodeGen => !OnlyThroughLexer && !OnlyThroughParser;
    public bool DoCodeEmission => !OnlyThroughLexer && !OnlyThroughParser && !OnlyThroughCodeGen;
    public bool DoAll => !(OnlyThroughLexer || OnlyThroughParser || OnlyThroughCodeGen || OnlyThroughCodeEmit);

    [Value(0, MetaName = "input file", HelpText = ".c file to compile", Required = true)]
    public required string InputFile { get; set; }

    public string BaseFilename => Regex.Replace(OutputFile ?? InputFile, @"\.\w+$", "");

    public string Text { get; set; } = "";

    public List<Token> TokenStream { get; set; } = [];

    public IAstNode Ast { get; set; } = null!;

    public TacProgram Tacky { get; set; } = null!;

    public IEnumerable<IAbstractAsm> AbstractInstructions { get; set; } = null!;

    public string Assembly { get; set; } = null!;
}