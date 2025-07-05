int main(void) {
    int a = 0;
    int b = 0;
    return (++a) + (b++);   // should return 1, though a will be 1
}