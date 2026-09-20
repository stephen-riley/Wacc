#!/usr/bin/env perl

# Run this from the root of the repo with `perl tmp/batch_create_tacky_gen_partial_records.pl`

use strict;
use warnings;
use v5.30;

foreach my $file (glob("src/Ast/*.cs")) {
    my $new_file = $file;
    $new_file =~ s{src/Ast}{src/Tacky/Ast};
    my( $class_name ) = $file =~ /src\/Ast\/(\w+)\.cs/;
    next if $class_name eq 'AstNode';

    open my $in, '<', $file or die "Could not open '$file' for reading: $!";
    open my $out, '>', $new_file or die "Could not open '$new_file' for writing: $!";
    say $out gen_class($class_name);
    close $in;
    close $out;
}

sub gen_class {
    my( $class_name ) = @_;
    my $var_name = lc(substr($class_name, 0, 1));

    return <<"END_CLASS";
using Wacc.Tacky;
using Wacc.Tacky.Instruction;
using static Wacc.Tacky.TackyGenerator;

namespace Wacc.Ast;

public partial record ${class_name}
{
    public override TacVal EmitTacky(TackyGenerator gen, AstNode node) => EmitTacky(gen, (${class_name})node);

    private TacVal EmitTacky(TackyGenerator gen, ${class_name} ${var_name})
    {
        throw new NotImplementedException("Tacky generation for ${class_name} is not implemented yet.");
    }
}
END_CLASS
}