//	配列
a[0,] = { 1, 2, 3 }
for (i = 0; i < arraySize(a[0,]); i++)
	print(a[0,i]," ");
/*
person["name",0] = "name";
person["name",1] = 18;
person["name",2] = "address";
*/
person["name",] = { "name", age, "address" }
size = arraySize(person["name",]);
print(size, "\n");
for (i = 0; i < size; i++)
	print(person["name",i],", ");
print();

//  if statement Test
a = 10;
b = 5;
if (a < b){
	print("a < b ");
	print(a, " ",b);
} else {
	print("a > b ");
	print(a, " ",b);
}