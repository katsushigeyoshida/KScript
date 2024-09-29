//	配列テスト
print("エラトステネスの篩\n");
print("最大数 >> ");
max = input();
m = 2;
while (m < sqrt(max)) {
    m = m + 1;
    print(m, " ");
}
print();
//return;
print("素数テーブル", "\n");
for (n = 2; n < max; n = n + 1) {
    a[n] = 1;
}
for (n = 2; n < max; n = n + 1) {
    if (a[n] == 1)
        print(n, " ");
}
print();
m = 2;
b = floor(sqrt(max));
print("最大値= ", max, " (", b, ")\n", "素数\n");
while (m < sqrt(max)) {
    if (a[m] == 1) {
        print(m," : ");
        for (n = m + m; n < max; n += m)
            a[n] = 0;
        for (n = 2; n < max; n++) {
            if (a[n] == 1)
                print(n, " ");
        }
        print();
    }
    m = m + 1;
}
