﻿//  Script Library
//  配列に加算関数
addArray(array[], add) {
    start = 0;
    size = arraySize(array[]);
    for (i = start; i < size; i = i + 1) {
        array[i] = array[i] + add;
    }
    return array[];
}
//  配列の平均
average(a[]) {
    s = sum(a[]);
    return s / arraySize(a[]);
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
//  分散
variance(a[]) {
    ave = average(a[]);
    size = arraySize(a[]);
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
    size = arraySize(array[]);
    for (i = start; i < size; i = i + 1) {
        print(array[i], " ");
    }
    print();
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
