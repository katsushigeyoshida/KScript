﻿//  TEST06
#include "scriptLib.sc";
b[] = { 1, 2, 3, 4 };
printArray(b[]);
c = sqrt(合計(b[]));
print(b, "^2 = ", 合計(b[]), " ", c, " ", 平方根(b[]), "\n");
平方根(b[]) {
    return sqrt(合計(b[]));
}
合計(a[]) {
    sum = 0;
    for (i = 0; i < arraySize(a[]); i++)
       sum += a[i];
    return sum;
}
