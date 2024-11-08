# KScript
## C言語風のスクリプト言語

比較的単純な構成で動くスクリプト言語(インタプリタ)を作成  
言語仕様は C に似せて見た。  
日本語変数、2次元配列、配列のインデックスに文字列が使えるなどの特徴がある。  

テスト環境はコンソール環境でできるようにスクリプトファイルを選択して実行する形にし、昔懐かしいラインエディタを組み込んでみた。  


### プログラムの構成
KLexer  :   字句解析  
KPerse  :   構文解析  
Script  :   スクリプトの実行処理  
Program :   テスト環境  

数式処理は CoreLib の YCalc.expression を呼び出す  

###  実行方法
    Script script = new Script();
    script.mScriptFolder = mScriptFolder;   //  #includeで読み込むファイルのフォルダ
    script.setScript(scriptCode);           //  スクリプトコードの読込(string)
    script.execute("main");                 //  スクリプトの実行



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
    数値と文字列を足すと文字列として結合する
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
    2次元配列
        a[1,2] = 10;
        a[,] = { { 1, 2, 3}, {10, 20, 30} }
        person["name",] = { "name", age, "address" }

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

※ 論理演算子と比較演算子が混在する場合、優先順位がつかないので括弧をつけて優先順位をつける  
    a > 10 && a < 20  →  (a > 10) && (a < 20)  

### 制御文
    if文     : 条件によって分岐させる
        if (条件) { 実行文; } [ else { 実行文; }]
    while文  : 条件に入っている間繰り返す
        while (条件) { 実行文; }
    for文    : 初期値、条件、増分を指定して繰り返す
        for (初期値; 条件; 増分) { 実行文; }
    break文  : ループの中断
    continue文   : ループの先頭に戻る
    exit文   : プログラムの終了

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

### 数式処理以外の関数(おもに配列関係)
    print(a, b, "\n");              : コンソールに文字出力  
    a = input();                    : コンソールからの入力  
    c = contain(a[x]);              : 配列の有無(0:なし 1:あり)
    size = count(a[]);              : 配列のサイズ
    size = count(a[,]);             : 2次元配列の全体の数
    size = count(a[1,]);            : 2次元配列(2行目)の列数
    max = max(a[]);                 : 配列の最大値
    min = min(a[,]);                : 配列の最小値
    sum = sum(a[]);                 : 配列の合計
    ave = average(a[,]);            : 配列の平均
    vari = variance(a[]);           : 分散
    std = stdDeviation(a[]);        : 標準偏差
    cov = covariance(a[], b[]);     : 共分散
    clear(a[]);                     : 配列をクリア
    remove(a[],st[,ed]);            : 配列の要素削除(1次元のみ)
    sort(a[]);                      : 配列のソート(1次元のみ)
    reverse(a[]);                   : 配列の逆順化(1次元のみ)
    a[,] = unitMatrix(size);        : 単位行列  
    d[,] = matrixTranspose(a[,]);   : 転置行列  
    d[,] = matrixMulti(a[,], b[,]); : 行列の積  
    d[,] = matrixAdd(a[,], b[,]);   : 行列の和  
    d[,] = matrixInverse(a[,]);     : 逆行列 a^-1  
    d[,] = copyMatrix(a[,]);        : 行列のコピー  
    dateTimeNow(type);              : 現在の時刻を文字列で取得  
    startTime();                    : 時間計測の開始  
    lapTime();                      : 経過時間の取得(秒)  
    solveQuadraticEquation(a,b,c);  : 2次方程式の解  
    solveCubicEquation(a,b,c,d);    : 3次方程式の解  
    solveQuarticEquation(a,b,c,d,e);: 4次方程式の解  

### 数式処理 : CoreLib の YCalc.expression(式) を呼び出す



### 履歴
2024/11/05 exit文の追加、時間関数,2次3次4次方程式の7関数追加  
2024/09/26 統計陽関数追加  
2024/09/21 #include文の追加  
2024/09/20 ラインエディタを持ったテスト環境作成
2024/09/12 マトリックス関数追加  
2024/08/20 2次元配列に対応  
2024/08/18 break,continue を追加  
2024/08/17 とりあえず動くもの  

### ■開発環境  
開発ソフト : Microsoft Visual Studio 2022  
開発言語　 : C# 10.0 Console アプリケーション  
フレームワーク　 :  .NET 8.0  
NuGetライブラリ : なし  
自作ライブラリ  : CoreLib (YCalc, YLib)  
  
