//	配列計算
abc[] = { 2, 3, 4, 5, 6, 7 }
printArray(abc[]);
total = sum(abc[]);
print("SumArray : ", total, "\n");
add = 2;
b[] = addArray(abc[], add);
printArray(b[]);

arrayClear(abc[]);
start = arrayStart(abc[]);
size = arraySize(abc[]);
print("start = ", start, " size = ", size, "\n");

//  配列に加算関数
addArray(array[], add) {
    start = arrayStart(array[]);
    size = arraySize(array[]);
    for (int i = start; i < size; i = i + 1) {
        array[i] = array[i] + add;
    }
    return array[];
}
//  配列表示
printArray(array[]) {
    start = arrayStart(array[]);
    size = arraySize(array[]);
    print("start = ", start, " size = ", size, "\n");
    for (int i = start; i < size; i = i + 1) {
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
