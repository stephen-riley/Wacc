        ; compiled from tests/chapter_3/valid/mod.c

        .section __TEXT,__text,regular,pure_instructions
        .align 2

        ; register aliases
        retval    .req w0
        scratch1  .req w9
        scratch2  .req w10
        scratch3  .req w11
        fp        .req w29
        lr        .req w30


        .global _main
_main:
        .cfi_startproc

        sub     sp, sp, #16             ; allocate stack frame
        stp     fp, lr, [sp, #16]       ; save FP and LR
        add     fp, sp, #16             ; set FP for new frame

        mov     scratch2, #4

        ; Fixup on Mov(2, Stack(-8))
        mov     scratch1, #2
        str     scratch1, [fp, #-8]
        ; 

        ; Fixup on Mod(Reg(scratch2), Stack(-8), Stack(-8))
        ldr     scratch1, [fp, #-8]
        sdiv    scratch3, scratch2, scratch1
        ;; msub    scratch1, scratch2, scratch3, scratch1
        msub    scratch1, scratch3, scratch1, scratch2
        str     scratch1, [fp, #-8]
        ; 

        ; Fixup on Mov(Stack(-8), Reg(retval))
        ldr     retval, [fp, #-8]
        ; 
        b       _Xmain

_Xmain:
        ldp     fp, lr, [sp, #16]
        add     sp, sp, #16
        ret
        .cfi_endproc
        ; end function _main

        ; program epilog here
