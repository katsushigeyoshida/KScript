//	function statement
arr[] = { 1,2,3,4 }
size = arraySize(arr[]);
print("サイズ ", size, "\n");
aa[] = addArray(arr[], 5);
for (i = 0; i < size; i++)
	print(aa[i], " ");
print();

for (i = 0; i < size; i++)
	print(arr[i], " ");
s = sum(arr[]);
print("合計 ", s);
print();

a = 1;
b = 2;
c = add(a, b);
print(a," ", b, " ", c, "\n");

addArray(a[], add) {
	size = arraySize(a[]);
	for (i = 0; i < size; i++) {
		a[i] += add;
	}
	return a[];
}

sum(a[]) {
	size = arraySize(a[]);
	sum = 0;
	for (i = 0; i < size; i++)
		sum += a[i];
	return sum;
}

add(a, b) {
	return a + b;
}
