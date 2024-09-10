//	while statetment Test
matSize = 4;
print("unitMatrix ", matSize)
a[,] = unitMatrix(matSize);
print(arraySize(a[,]), "\n");
for (i = 0; i < 3; i++) {
	for (j = 0; j <3; j++) {
		print(a[i,j], " ");
	}
	print();
}
url = "https://github.com/katsushigeyoshida";
cmd(url);
print("cmd ",url)

print("while statement Test\n");"
a = 0;
b = 2;
while (a + 2 <= 5 + b) {
	print(a, " ");
	a += 2;
}
print();

print("for statement Test\n");"
for (i = 1; i < 3; i++)
	print(i, " ");
print();
for (i = 2; i < 100; i *= 2) {
	print(i, " ");
}
print();
