# KScript
## C言語風のスクリプト言語

比較的単純な構成で動くスクリプト言語を作成  
言語仕様は C に似せて見た。  

### プログラムの構成
KLexer  :   字句解析  
KPerse  :   構文解析  
Program :   テスト  
数式処理は CoreLib の YCalc.expression を呼び出す  

### プロクラムの構造
    main() {                        main関数(省略可)
        a = 3;                      代入文
        b = 2;
        c = add(a, b);              関数呼び出し
        print(a + b, " ", c);
    }
    add(c, d) {                     関数定義
        return c + d;
    }

### 文の書き方
    一つの文はセミコロン(;)で終了する

### コメント
    // から行末までをコメントアウトする
    /* から */ もコメントアウトするこの場合は複数行になってもかまわない

### 変数
    値を格納するもので特に型指定の必要はなくて値としては実数と文字列が使える
    数値と文字列を足すと文字列を結合する
    a = 3;
    b = "abcd";
    c = a + b;        → "3ABC"

### 配列
    複数のデータをまとめて扱うもので[ ]の中の数値を変えて値を格納する
    a[0] = 10;
    a[1] = 20;
    a[2] = 30;
    値をまとめて設定するときは
    a[] = { 10, 20, 30 }

### 演算子
    数式を処理するための演算子
    +,-,*,/,%,^ : 加算,減算,乗算,除算,剰余,べき乗
### 複合演算子
    2つの演算子を組み合わせて演算をおこなう
    +=,^=,*=,/=,%=,^=,++,--
    a += 2;  →  a = a + 2;
    a++;  →  a = a + 1;
### 比較演算子
    条件部の中で２つの値を比較する演算子
    ==, !=, <, >, <=, >=　: 等しい,等しくない,小さい,大きい,小さいか等しい,大きいか等しい
    if (a == b) print(a);
### 論理演算子
    比較演算子の結果を演算する
    &&,||,! : aかつb , aまたはb , aではない

### 制御文
    if文     : 条件によって分岐させる
        if (条件) { 実行文; } [ else { 実行文; }]
    while文  : 条件に入っている間繰り返す
        while (条件) { 実行文; }
    for文    : 初期値、条件、増分を指定して繰り返す
        for (初期値; 条件; 増分) { 実行文; }

### 関数の定義
    複数の文をまとめて実行する仕組み
    関数名(引数1, ...) { 実行文; ... return 返値; }
        add(a, b) {
            c = a + b;
            return c;
        }
    呼出し側
        s = 2;
        t = 3;
        sum = add(s, t);
    引数に配列を使う場合
        sum(a[]) {
            size = arraySize(a[]);
            sum = 0;
            for (i = 0; i < size; i++) {
                sum += a[i];
            }
            return sum;
        }
    呼出し側
        array[] = { 1, 2, 3, 4, 5 }
        total = sum(array[]);

### 数式処理以外の関数
    print(a, b, "\n");          : コンソールに文字出力
    a = input();                : コンソールからの入力
    start = arrayStart(a[]);    : 配列の開始位置
    size = arraySize(a[]);      : 配列のサイズ
    arrayClear(a[]);            : 配列をクリア

### 数式処理 : YCalc.expression(式) を呼び出す


…or create a new repository on the command line

echo "# KScript" >> README.md
git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/katsushigeyoshida/KScript.git
git push -u origin main

…or push an existing repository from the command line

git remote add origin https://github.com/katsushigeyoshida/KScript.git
git branch -M main
git push -u origin main
