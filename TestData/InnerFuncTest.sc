//	while statetment Test
print("マトリックステスト\n");
matSize = 4;
print("unitMatrix ", matSize)
a[,] = unitMatrix(matSize);
print("\na[,] size : ", count(a[,]), "\n");
printArray2(a[,]);
print("matrixAdd\n");
b[,] = unitMatrix(matSize);
c[,] = matrixAdd(a[,], b[,]);
printArray2(c[,]);
/*
print("コマンド実行テスト\n");
url = "https://github.com/katsushigeyoshida";
cmd(url);
print("cmd ",url)
*/
print("while statement Test\n");
a = 0;
b = 2;
while (a + 2 <= 5 + b) {
    print(a, " ");
    a += 2;
}
print();
print("for statement Test\n");
for (i = 1; i < 3; i++) {
    print(i, " ");
}
print();
for (i = 2; i < 100; i *= 2) {
    print(i, " ");
}
print();
//  2D配列表示
printArray2(array[,]) {
    size = count(array[,]);
    print("サイズ : ",size, "\n");
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = count(array[i,]);
        print(i, " : ");
        for (j = 0; j < rowsize; j++) {
            print(array[i,j], " ");
            count++;
        }
        print();
        i++;
    }
    print();
}
