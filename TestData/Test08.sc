//	while statetment Test
a[,] = unitMatrix(3);
print(arraySize(a[,]), "\n");
for (i = 0; i < 3; i++) {
	for (j = 0; j <3; j++) {
		print(a[i,j], " ");
	}
	print();
}
url = "https://github.com/katsushigeyoshida";
cmd(url);
a = 0;
b = 2;
while (a + 2 <= 5 + b) {
	print(a, " ");
	a += 2;
}
print();
//  for statement Test
for (i = 1; i < 3; i++)
	print(i, " ");
print();
for (i = 2; i < 100; i *= 2) {
	print(i, " ");
}
print();
