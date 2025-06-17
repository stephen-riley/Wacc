#ifdef SUPPRESS_WARNINGS
#ifndef __clang__
#pragma GCC diagnostic ignored "-Wparentheses"
#endif
#endif

int main(void) {
    // 5 >= 0 > 1 <= 0
    // (1) > 1 <= 0
    // (0) <= 0
    // 1
    return 5 >= 0 > 1 <= 0;
}