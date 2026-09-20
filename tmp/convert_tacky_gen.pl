#!/usr/bin/env perl

# Run this from the root of the repo with `perl tmp/batch_create_tacky_gen_partial_records.pl`

use strict;
use warnings;
use v5.30;

use File::Slurp;

my @cases;

mkdir 'src/Tacky/Generator' unless -d 'src/Tacky/Generator';

foreach my $file (glob("src/Tacky/Ast/*.cs")) {
    my( $class_name ) = $file =~ /src\/Tacky\/Ast\/(\w+)\.cs/;
    next if $class_name eq 'AstNode';

    my $class_var_name = lc( $class_name =~ s/[a-z]//gr );

    my $source_code = read_file($file) =~ s/.*private TacVal EmitTacky.*?\n//sr;
    $source_code =~ s/gen\.//gs;

    push @cases, "case $class_name $class_var_name: return EmityTackyFor${class_name}(${class_var_name});";

    my $new_path = "src/Tacky/Generator/$class_name.cs";
    open my $out, '>', $new_path or die "Could not open '$new_path' for writing: $!";
    my $new_header = <<"END_HEADER";
using Wacc.Ast;
using Wacc.Exceptions;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Tacky;

public partial class TackyGenerator
{
    private TacVal EmityTackyFor${class_name}(${class_name} ${class_var_name})
END_HEADER
    $new_header =~ s/\n+$//s;
    say $out "$new_header\n$source_code";
    close $out;
}

say foreach (@cases);
