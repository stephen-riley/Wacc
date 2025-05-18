#! /usr/bin/env perl

# Constructs a static factory class for AbstractAsm classes.
# 
# Invoke from repo root as /tmp/build_asm_factory.pl

use strict;
use warnings;
use v5.30;

use File::Slurp;

say STDERR "Building src/CodeGen/AbstractAsm/Asm.cs factory class--";

open OUT, '>', 'src/CodeGen/AbstractAsm/AF.cs' or die "$!\n";
print OUT << 'PROLOG';
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen.AbstractAsm;

public static class AF
{
PROLOG

my @files = <src/CodeGen/AbstractAsm/Instruction/*.cs>;
push @files, <src/CodeGen/AbstractAsm/Operand/*.cs>;


for my $f ( @files ) {
    next if $f =~ /Asm\.cs$/;

    my $cs = read_file( $f );
    $cs =~ s/[\{\}\[\]]//g;
    next if $cs =~ /abstract record/;
    next if $cs !~ /record/;

    my @params;
    my $param_names;

    my( $name, $params ) = $cs =~ /public record Asm(\w+)\((.*?)\)/s;
    if( defined $params ) {
        @params = map { s/ = .*//r } map { substr( $_, index( $_, ' ')+1 ) } split( /, /, $params );
        $param_names = join( ', ', @params );
    } else {
        $param_names = '';
    }

    my $count = 1;
    say OUT "    public static Asm$name $name($params) => new($param_names);";

    my( $alt_name, $alt_constructor_param ) = $cs =~ /public Asm(\w+)\((.*?)\) : this/m;
    if( $alt_constructor_param && $alt_name eq $name ) {
        my( $type, $pname ) = split( / /, $alt_constructor_param );
        say OUT "    public static Asm$name $name($alt_constructor_param) => new($pname);";
        $count++;
    }

    say STDERR "Wrote $count factory constructor(s) for Asm$name";
}
say OUT << 'EPILOG';

    public static AsmInstruction Create(Type type, AsmOperand src1, AsmOperand src2, AsmOperand dst)
    {
        var i = Activator.CreateInstance(type, src1, src2, dst) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmInstruction Create(Type type, AsmOperand src, AsmOperand dst)
    {
        var i = Activator.CreateInstance(type, src, dst) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmInstruction Create(Type type, AsmOperand src)
    {
        var i = Activator.CreateInstance(type, src) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmInstruction Create(Type type)
    {
        var i = Activator.CreateInstance(type) ?? throw new CodeGenError($"{nameof(AF)}.{nameof(Create)}: cannot create instance of type {type.Name}");
        return (AsmInstruction)i;
    }

    public static AsmRegOperand FP => new(ArmReg.FP);

    public static AsmRegOperand SP => new(ArmReg.SP);

    public static AsmRegOperand SCRATCH1 => new(ArmReg.SCRATCH1);

    public static AsmRegOperand SCRATCH2 => new(ArmReg.SCRATCH2);

    public static AsmRegOperand SCRATCH3 => new(ArmReg.SCRATCH3);

    public static AsmRegOperand RETVAL => new(ArmReg.RETVAL);
}
EPILOG

close OUT;

say STDERR "Done.";