﻿//  配列
a[0,] = { 1, 2, 3, 4 }
for (i = 0; i < count(a[0,]); i++)
    print(a[0,i]," ");
print();
exit;
person["name",] = { "name", age, "address" }
size = count(person["name",]);
print("personal size : ",   size, "\n");
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
