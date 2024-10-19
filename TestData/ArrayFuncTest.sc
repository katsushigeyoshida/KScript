//	配列計算
print("2次元配列\n");
a[,] = {
    {  1,  2,  3,  4,  5 }, 
    {  6,  7,  8,  9, 10 },
    { 11, 12, 13, 14, 15 }
};
printArray2(a[,]);
print();
print("配列の合計、平均\n")
x[] = { 2, 4, 7, 8, 9 }
y[] = { 30,50, 70, 40,80 }
printArray(x[]);
total = sum(x[]);
ave = average(x[]);
vari = variance(x[]);
print("合計 : ", total, " 平均: ", ave, "\n");
sd = stdDev(x[]);
print(" 分散: ", vari, " 標準偏差: ", sd, "\n");
print("配列の加算\n")
abc[] = { 2, 4, 7, 8, 9 }
printArray(abc[]);
print("abc[] + 2\n")
add = 2;
b[] = addArray(abc[], add);
printArray(b[]);
print("配列クリア\n");
clear(abc[]);
start = 0;
size = count(abc[]);
print("start = ", start, " size = ", size, "\n");

//  配列に加算関数
addArray(array[], add) {
    start = 0;
    size = count(array[]);
    for (i = start; i < size; i = i + 1) {
        array[i] = array[i] + add;
    }
    return array[];
}
//  配列の平均
average(a[]) {
    s = sum(a[]);
    return s / count(a[]);
}
//  配列の合計
sum(a[]) {
    size = count(a[]);
    sum = 0;
    for (i = 0; i < size; i++) {
        sum += a[i];
    }
    return sum;
}
//  分散
variance(a[]) {
    ave = average(a[]);
    size = count(a[]);
    sum = 0;
    for (i = 0; i < size; i++) {
        sum += (a[i] - ave)^2;
    }
    return sum / size;
}
//  標準偏差standard deviation SD)
stdDev(a[]) {
    //b = variance(a[]);
    return sqrt(variance(a[]));
}
//  配列表示
printArray(array[]) {
    start = 0;
    size = count(array[]);
    for (i = start; i < size; i = i + 1) {
        print(array[i], " ");
    }
    print();
}
//  2D配列表示
printArray2(array[,]) {
    size = count(array[,]);
    print("サイズ : ",size, "\n");
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = count(array[i,]);
        print(rowsize, " : ");
        for (j = 0; j < rowsize; j++) {
            print(array[i,j], " ");
            count++;
        }
        print();
        i++;
    }
    print();
}
