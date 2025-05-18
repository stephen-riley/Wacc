/*
    NOTE: this one may go away if the book wants us to support cpp
*/

#ifdef SUPPRESS_WARNINGS
#ifdef __clang__
#pragma clang diagnostic ignored "-Wbitwise-op-parentheses"
#else
#pragma GCC diagnostic ignored "-Wparentheses"
#endif
#endif

int main(void) {
    return 80 >> 2 | 1 ^ 5 & 7 << 1;
}