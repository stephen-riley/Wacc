using System.Text.RegularExpressions;
using CommandLine;
using Wacc.Tacky.Instruction;
using Wacc.Tokens;
using Wacc.CodeGen.AbstractAsm.Instruction;

namespace Wacc;

[Verb("cc", isDefault: true, HelpText = "Invoke compiler (default)")]
public class RuntimeState
{
    [Option('l', "lex", HelpText = "Run the lexer, but stop before parsing")]
    public bool OnlyThroughLexer { get; set; } = false;

    [Option('p', "parse", HelpText = "Run the lexer and parser, but stop before assembly generation")]
    public bool OnlyThroughParser { get; set; } = false;

    [Option('v', "validate", HelpText = "Run semantic analysis before TACKY generation")]
    public bool OnlyThroughValidate { get; set; } = false;

    [Option('t', "tacky", HelpText = "Perform lexing, parsing, and assembly generation, but stop before code emission")]
    public bool OnlyThroughTacky { get; set; } = false;

    [Option('c', "codegen", HelpText = "Perform lexing, parsing, and assembly generation, but stop before code emission")]
    public bool OnlyThroughCodeGen { get; set; } = false;

    [Option('e', "emit", HelpText = "Perform lexing, parsing, assembly generation, and code emission")]
    public bool OnlyThroughCodeEmit { get; set; } = false;

    [Option("verbose", HelpText = "Verbose output on STDERR")]
    public bool Verbose { get; set; }

    [Option('o', "output", HelpText = "Output exe filename")]
    public string? OutputFile { get; set; }

    [Option('O', "optimize", HelpText = "Optimization level (0=none)")]
    public int Optimize { get; set; } = 0;

    [Option('S', "assemble", HelpText = "Output assembly file and stop")]
    public bool Assemble { get; set; }

    [Option("asmfile", HelpText = "Save assembly file to filename")]
    public string? AsmFilename { get; set; }

    [Option("asm-include-comments", HelpText = "Include compiler-generated comments in assembly output")]
    public bool AsmIncludeComments { get; set; }

    public bool DoLexer { get; } = true;
    public bool DoParser => !OnlyThroughLexer;
    public bool DoValidate => !OnlyThroughLexer && !OnlyThroughParser;
    public bool DoTacky => !OnlyThroughLexer && !OnlyThroughParser && !OnlyThroughValidate;
    public bool DoCodeGen => !OnlyThroughLexer && !OnlyThroughParser && !OnlyThroughValidate && !OnlyThroughTacky;
    public bool DoCodeEmission => !OnlyThroughLexer && !OnlyThroughParser && !OnlyThroughValidate && !OnlyThroughTacky && !OnlyThroughCodeGen;
    public bool DoAll => !(OnlyThroughLexer || OnlyThroughParser || OnlyThroughValidate || OnlyThroughTacky || OnlyThroughCodeGen || OnlyThroughCodeEmit || Assemble);

    [Value(0, MetaName = "input file", HelpText = ".c file to compile", Required = true)]
    public required string InputFile { get; set; }

    public string BaseFilename => Regex.Replace(OutputFile ?? InputFile, @"\.\w+$", "");

    public string Text { get; set; } = "";

    public List<Token> TokenStream { get; set; } = [];

    public Ast.Program Ast { get; set; } = null!;

    public bool Validated { get; set; } = false;

    public TacProgram Tacky { get; set; } = null!;

    public IEnumerable<AsmInstruction> AbstractAsm { get; set; } = null!;

    public string Assembly { get; set; } = null!;
}