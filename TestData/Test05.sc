//  ソート
data[] = { 6, 9, 12, 7, 2, 23, 10, 4 };
print("ソートデータ\n");
arrayPrint(data[]);
print("1:単純選択法\n");
print("2:バブルソート\n");
print("3:単純挿入法\n");
sortNo = input();
if (sortNo == 1) simpleSelect(data[]);
if (sortNo == 2) bubleSort(data[]);
if (sortNo == 3) simpleInsert(data[]);

simpleSelect(a[]) {
    print("単純選択法\n");
    sp = arrayStart(a[]);
    ep = arraySize(a[]);
    n = ep;
    for ( i = sp; i < n; i = i + 1) {
        k = sp;
        for (j = sp; j < n; j = j + 1) {
            if (a[k] > a[j]) k = j;
        }
        b[i] = a[k];
        a[k] = 9999;
        arrayPrint(b[]);
    }
}

bubleSort(a[]) {
    print("バブルソート\n");
    sp = arrayStart(a[]);
    ep = arraySize(a[]);
    for ( i = sp; i < ep; i = i +1){
        for ( j = sp; j < ep - 1; j = j + 1) {
            if (a[j] > a[j + 1]){
                w = a[j];
                a[j] = a[j + 1];
                a[j + 1] = w;
            }
        }
        arrayPrint(a[]);
    }
}

simpleInsert(a[]) {
    print("単純挿入法(1列目作業データ)\n");
    sp = arrayStart(a[]);
    ep = arraySize(a[]);
    for ( i = sp; i < ep; i = i +1){
        b[sp] = a[i];
        k = i;
        while(b[sp] < b[k]) {
            b[k + 1] = b[k];
            k = k - 1;
        }
        b[k + 1] = b[sp];
        arrayPrint(b[]);
    }
}

arrayPrint(b[]) {
    i = arrayStart(b[]);
    ep = arraySize(b[]);
    while (i < ep) {
        print(b[i], " ");
        i = i + 1;
    }
    print();
}
