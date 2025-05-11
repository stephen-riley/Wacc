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
print OUT << 'EOC';
using Wacc.CodeGen.AbstractAsm.Instruction;
using Wacc.CodeGen.AbstractAsm.Operand;
using Wacc.Tacky.Instruction;

namespace Wacc.CodeGen.AbstractAsm;

public static class AF
{
EOC

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
        @params = map { substr( $_, index( $_, ' ')+1 ) } split( /, /, $params );
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
say OUT "}";

close OUT;

say STDERR "Done.";