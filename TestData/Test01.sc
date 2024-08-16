// Test Program
print("増分(m) = ");
m = input();
for (i = 1; i < 100; i += m) {
    print(i, " ")
}
print();
m ^= 2;
print("m^2 = ", m, "\n");
a = 3;
b = "ABC";
c = a + b;
print("3 + ABC = ", c);
//  Test Function
add(a, b) {
    return a + b;
}
