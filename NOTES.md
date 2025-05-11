# Chapter 2

## Replacing pseudo-registers

The goal here is to replace all `Pseudo("tmp.N")` (`AsmPseudoOperand`) operands with `Stack(offset)` (`AsmStackOperand`) operands.  Later we'll fix up instructions that are of the `Move(Stack(x), Stack(y))` variety to account for Arm being a load-store ISA.

The basic pseudo-code looks like this:

```
for each instruction i:
    if has a Src and it's a Pseudo("var"):
        get offset for "var" in the local vars lookup table
        replace i.Src with Stack(offset)
    
    if i has a Dst and it's a Pseudo("var"):
        do the same thing

    otherwise:
        just pass the instruction through
```

## Fixing up instructions