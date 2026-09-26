int main(void) {
    int a = 3;
    int x = 0;

    switch (a) {
        x = 12;
        case 0: x = 0;
        case 1: x = 1; break;
        case 2: x = 2;
        default: x = 100;
    }

    return x;   // should return 100
}