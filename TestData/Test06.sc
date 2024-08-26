// let Test
n = 2;
i = n;
i += n;
print(i, " ", n,", ");
print();
max = 20;
for (i = n; i < max; i += n) {
    print(i, " ", n,", ");
}
print();

a = 5;
print(a, " ");
b[1] = 5 + 3;
print(b[1], " ");
b[1] += 2;
print(b[1], " ");
print();

print("1次元配列\n");
b[] = { 1, 2, 3, 4 }
size = arraySize(b[]);
print(size, ": ", b[0], " ",b[1], " ",b[2], " ",b[3], " ");
print();
print("2次元配列\n");
a[,] = {
    { 1, 2, 3, 4, 5 }, 
    { 6, 7+1, 8*2, 9, 10, 11 }
};
print("配列サイズ: ", arraySize(a[,])," ", arraySize(a[0,])," ", arraySize(a[1,]),"\n");

for (i = 0; i < 2; i++) {
    for (j = 0; j < arraySize(a[i,]); j++) {
        print(a[i,j], " ");
    }
    print();
}

print("文字配列\n");
fruit["apple"] = "red";
fruit["remmon"] = "yellow";
print(fruit["apple"], "\n");
m = "remmon";
print(fruit[m], "\n");
print();
c[] = { 1, "two", "three", 4 }
for (i = 0; i < 4; i++)
	print(c[i], " ");
print();
