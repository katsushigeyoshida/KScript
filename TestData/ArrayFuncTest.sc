//	配列計算
print("2次元配列\n");
a[,] = {
    {  1,  2,  3,  4,  5 }, 
    {  6,  7,  8,  9, 10 },
    { 11, 12, 13, 14, 15 }
};
printArray2(a[,]);
print();
abc[] = { 2, 3, 4, 5, 6, 7 }
printArray(abc[]);
total = sum(abc[]);
print("SumArray : ", total, "\n");
add = 2;
b[] = addArray(abc[], add);
size = arraySize(b[]);
print("size ",size, " ");
for (i = 0; i < size; i++)
	print(b[i], " ");
print();

printArray(b[]);

arrayClear(abc[]);
start = 0;
size = arraySize(abc[]);
print("start = ", start, " size = ", size, "\n");

//  配列に加算関数
addArray(array[], add) {
    start = 0;
    size = arraySize(array[]);
    for (i = start; i < size; i = i + 1) {
        array[i] = array[i] + add;
    }
    return array[];
}
//  配列表示
printArray(array[]) {
    start = 0;
    size = arraySize(array[]);
    print("start = ", start, " size = ", size, "\n");
    for (i = start; i < size; i = i + 1) {
        print(array[i], " ");
    }
    print();
}
//  配列の合計
sum(a[]) {
    size = arraySize(a[]);
    sum = 0;
    for (i = 0; i < size; i++) {
        sum += a[i];
    }
    return sum;
}
//  2D配列表示
printArray2(array[,]) {
    size = arraySize(array[,]);
    print("サイズ : ",size, "\n");
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = arraySize(array[i,]);
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
