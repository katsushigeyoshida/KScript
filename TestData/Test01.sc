// Test Program
key[] = { "Apple", "Orenge" }
print(key[0], "\n");
for (i = 0; i < 2; i++)
    print(key[i]," ");
print();
fruit["Apple"] = 20;
fruit["Orenge"] = 50;
print(fruit["Orenge"], " ", fruit["Apple"],"\n")

print("2次元配列\n");
a[,] = {
    {1,2,3,4,5}, 
    {6,7,8,9,10}
};
print("配列サイズ: ", arraySize(a[0,]), "\n");
for (i = 0; i < 2; i++) {
    for (j = 0; j < 5; j++) {
        print(a[i,j], " ");
    }
    print();
}
print("配列の有無\n");
for (i = 0; i < 10; i++) {
    print(i, " ", a[1,i]," [", arrayContain(a[1,i]),"] ");
}
print();

print("1次元配列\n");
b[] = { 1, 2, 3, 4, 5 }
for (i = 0; i < arraySize(b[]); i++) {
    print(b[i], " ");
}
print();
print("2次元配列\n");
for(m = 0; m < 3; m++) {
    for(n = 0; n < 4; n++) {
        a[m, n] = m * 10 + n;
    }
}
for(m = 0; m < 3; m++) {
    for(n = 0; n < 4; n++) {
        print(a[m, n], " ");
    }
    print();
}
print();

