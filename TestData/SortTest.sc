//  ソート
data[] = { 6, 9, 12, 7, 2, 23, 10, 4 };
print("ソートデータ\n");
arrayPrint(data[]);
print("1:単純選択法\n");
print("2:バブルソート\n");
print("3:単純挿入法\n");
print("4:シェーカーソート\n");
print("5:単純挿入法2\n");
print("6: シェルソート\n");
print("7: クイックソート\n");
sortNo = input();
arrayPrint(data[]);
if (sortNo == 1) simpleSelect(data[]);
else if (sortNo == 2) bubleSort(data[]);
else if (sortNo == 3) simpleInsert(data[]);
else if (sortNo == 4) shakerSort(data[]);
else if (sortNo == 5) simpleInsert2(data[]);
else if (sortNo == 6) shellSort(data[]);
else if (sortNo == 7) quickSort(data[]);
simpleSelect(a[]) {
    sp = 0;
    ep = count(a[]);
    print("単純選択法 ",ep, "\n");
    count = 0;
    n = ep;
    for ( i = sp; i < n; i = i + 1) {
        k = sp;
        for (j = sp; j < n; j = j + 1) {
            count++;
            if (a[k] > a[j]) k = j;
        }
        b[i] = a[k];
        a[k] = 9999;
        print("[",count,"] ")
        arrayPrint(b[]);
    }
}
bubleSort(a[]) {
    print("バブルソート\n");
    sp = 0;
    ep = count(a[]);
    count = 0;
    for ( i = sp; i < ep; i = i +1){
        for ( j = sp; j < ep - 1; j = j + 1) {
            count++;
            if (a[j] > a[j + 1]){
                w = a[j];
                a[j] = a[j + 1];
                a[j + 1] = w;
            }
        }
        print("[",count,"] ")
        arrayPrint(a[]);
    }
}
simpleInsert(a[]) {
    print("単純挿入法(1列目作業データ)\n");
    sp = 0;
    ep = count(a[]);
    count = 0;
    for ( i = sp; i < ep; i = i +1){
        count++;
        b[sp] = a[i];
        k = i;
        while(b[sp] < b[k]) {
            count++;
            b[k + 1] = b[k];
            k = k - 1;
        }
        b[k + 1] = b[sp];
        print("[",count,"] ")
        arrayPrint(b[]);
    }
}
shakerSort(a[]) {
    print("シェーカーソート\n");
    n = count(a[]);
    count = 0;
    shift = 0;
    left = 0;
    right = n - 1;
    while (left < right) {
        for (i = left; i < right; i++) {
            count++;
            if (a[i] > a[i+1]) {
                t = a[i];
                a[i] = a[i+1];
                a[i+1] = t;
                shift = i;
            }
        }
        print("[",count,"] ")
        arrayPrint(a[]);
        right = shift;
        for (i = right; i > left; i--) {
            count++;
            if (a[i] < a[i-1]) {
                t = a[i];
                a[i] = a[i-1];
                a[i-1] = t;
                shift = i;
            }
        }
        left = shift;
        print("[",count,"] ")
        arrayPrint(a[]);
    }
}
simpleInsert2(a[]) {
    print("単純挿入ソート\n");
    n = count(a[]) - 1;
    count = 0;
    for (i = 1; i < n; i++) {
        for (j = i - 1; j >= 0; j--) {
            count++;
            if (a[j] > a[j+1]) {
                t = a[j];
                a[j] = a[j+1];
                a[j+1] = t;
            } else {
                break;
            }
        }
        print("[",count,"] ")
        arrayPrint(a[]);
    }
}
shellSort(a[]) {
    print("シェルソート\n");
    n = count(a[]) - 1;
    gap = floor(n / 2);
    count = 0;
    while (gap > 0) {
        for (k = 0; k < gap; k++) {
            for (i = k + gap; i < n; i+=gap) {
                for (j = i - gap; j >= k; j-=gap) {
                    count++;
                    if (a[j] > a[j+gap]) {
                        t = a[j];
                        a[j] = a[j+gap];
                        a[j+gap] = t;
                    } else {
                        break;
                    }
                }
            }
            print("[",count,"] ")
            arrayPrint(a[]);
        }
        gap = floor(gap / 2);
    }
}

quickSort(a[]) {
    println("クイックソート");
	n = count(a[]) - 1;
	a[] = quick(a[], 0, n);
}

quick(a[], left, right) {
	if (left < right) {
		print(left, " ", right, " : ");
		arrayPrint(a[]);
		s = a[left];
		i = left;
		j = right + 1;
		while (1) {
			while (a[++i] < s) ;
			while (a[--j] > s) ;
			if (i >= j) break;
			t = a[i];
			a[i] = a[j];
			a[j] = t;
		}
		a[left] = a[j];
		a[j] = s;
		
		a[] = quick(a[], left, j - 1);
		a[] = quick(a[], j + 1, right);
	}
	return a[];
}

arrayPrint(b[]) {
    i = 0;
    ep = count(b[]);
    while (i < ep) {
        print(b[i], " ");
        i = i + 1;
    }
    print();
}
