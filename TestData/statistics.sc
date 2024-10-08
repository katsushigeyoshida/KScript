﻿//  統計(配列)のテスト
#include "scriptLib.sc";
a[] = { 1, 2, 13, -4, 5 };
print("配列\n");
printArray(a[]);
print("max = ", max(a[]), " min = ", min(a[]), " sum = ", sum(a[]), " average = ", average(a[]), "\n");
b[,] = { { 10, 3, 14, 5, 9 }, { 1, 2, 8, 2, 0 } };
print("2次元配列\n");
printArray2(b[,]);
max = max(b[0,]);
print("max = ", max(b[,]), " ", max(b[0,]), " ", max(b[1,]), "\n" );
print("min = ", min(b[,]), " ", min(b[0,]), " ", min(b[1,]), "\n" );
print("sum = ", sum(b[,]), " ", sum(b[0,]), " ", sum(b[1,]), "\n" );
print("average = ", average(b[,]), " ", average(b[0,]), " ", average(b[1,]), "\n" );
matui[] = { 10000, 3000, 1000, 1000, 15000, 0 };
yamada[] = { 3000, 7000, 2000, 8000, 4000, 6000};
//for (i = 0; i < count(matui[]); i++) {
//    shoukei[i] = matui[i] + yamada[i];
//}
shoukei[] = { 25000000, 40000000, 20000000, 55000000, 35000000, 45000000 };
print("matui  : "); printArray(matui[]);
print("yamada : "); printArray(yamada[]);
print("shoukei: "); printArray(shoukei[]);
print("shoukeiの合計 ", sum(shoukei[]), "\n");
print("matui  : ", sum(matui[]), " ", variance(matui[]), " ", stdDeviation(matui[]), " ", covariance(matui[], shoukei[]), "\n");
print("yamada : ", sum(yamada[]), " ", variance(yamada[]), " ", stdDeviation(yamada[]), " ", covariance(yamada[], shoukei[]), "\n"); 
//
x[] = {117, 100.5, 120.3, 115.5, 125.4 };
y[] = { 21.8, 19.5, 23.5, 20.5, 24.5 };
printArray(x[]);
printArray(y[]);
print("共分散  : ", covariance(x[], y[]), "\n");
print("相関係数: ", corrCoeff(x[], y[]), "\n");
