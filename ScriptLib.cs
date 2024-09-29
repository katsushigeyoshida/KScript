using CoreLib;

namespace KScript
{
    /// <summary>
    /// 追加内部関数
    ///     input    : a = input();                         キー入力(文字列)
    ///     contains : a = contains(c[2]);                  配列の有無(0:なし 1:あり)
    ///     count    : size = count(a[]);                   1次元配列のサイズ
    ///                size = count(b[,]);                  2次元配列のサイズ
    ///                size = count(b[2,]);                 2次元配列1列目のサイズ
    ///     max      : max = max(a[]);                      配列の最大値
    ///     min      : min = min(a[,]);                     配列の最小値
    ///     sum      : sum = sum(a[]);                      配列の合計
    ///     average  : ave = average(a[,]);                 配列の平均
    ///     variance    : vari = variance(a[]);             分散
    ///     stdDeviation: std = stdDeviation(a[]);          標準偏差
    ///     covariance  : cov = covariance(x[],y[]);        共分散
    ///     corrCoeff   : corr = corrCoeff(x[],y[]);        相関係数
    ///     clear    : clear(a[]);                          配列クリア
    ///     remove   : remove(a[],st[,ed]);                 配列要素の削除
    ///     sort     : sort(a[]);                           ソート
    ///     reverse  : reverse(a[]);                        逆順
    ///     cmd      : cmd(command);                        Windowsコマンドの実行
    ///     
    ///     unitMatrix      : a[,] = unitMatrix(size);          単位行列の作成
    ///     matrixTranspose : b[,] = matrixTranspose(a[,]);     転置行列  行列Aの転置A^T
    ///     matrixMulti     : c[,] = matrixMulti(a[,], b[,]);   行列の積 AxB
    ///     matrixAdd       : c[,] = matrixAdd(a[,], b[,]);     行列の和 A+B
    ///     matrixInverse   : b[,] = matrixInverse(a[,]);       逆行列 A^-1
    ///     copyMatrix      : b[,] = copyMatrix(a[,]);          行列のコピー
    ///     
    /// 追加したい関数
    ///     key  : a = key();                               1文字入力
    ///     load : text = load(path);                       テキストファイルの読込み
    ///     save : save(path, text);                        テキストファイルの書き込み
    ///     graph                                           グラフ表示(Win版)
    ///     chart                                           チャート表示(Win版)
    ///     table                                           表編集(Win版)
    ///     imageView                                       イメージ表示(Win版)
    ///     
    /// </summary>
    public class ScriptLib
    {
        public Dictionary<string, Token> mVariables;    //  変数リスト(変数名,値)

        private Script mScript;
        private KLexer mLexer = new KLexer();
        private YCalc mCalc = new YCalc();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script">Scriptクラス</param>
        /// <param name="variables">変数テーブル(KParse.mVariable)</param>
        public ScriptLib(Script script, Dictionary<string, Token> variables)
        {
            mScript = script;
            mVariables = variables;
        }

        /// <summary>
        /// 追加内部関数
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>戻り値</returns>
        public Token innerFunc(Token funcName, Token arg, Token ret)
        {
            string argValue = mLexer.stripBracketString(arg.mValue);
            Token args = getValueToken(argValue);
            switch (funcName.mValue) {
                case "input"          : return new Token(Console.ReadLine(), TokenType.STRING);
                case "contains"       : return contains(args);
                case "count"          : return getCount(args);
                case "clear"          : clear(args); break;
                case "remove"         : remove(args); break;
                case "squeeze"        : squeeze(args); break;
                case "sort"           : sort(args); break;
                case "reverse"        : reverse(args); break;
                case "max"            : return max(args);
                case "min"            : return min(args);
                case "sum"            : return sum(args);
                case "average"        : return average(args);
                case "variance"       : return variance(args);
                case "stdDeviation"   : return standardDeviation(args);
                case "covariance"     : return covariance(args);
                case "corrCoeff"      : return correlationCoefficient(args);
                case "cmd"            : cmd(args); break;
                case "unitMatrix"     : return unitMatrix(args, ret);
                case "matrixTranspose": return matrixTranspose(args, ret);
                case "matrixMulti"    : return matrixMulti(args, ret);
                case "matrixAdd"      : return matrixAdd(args, ret);
                case "matrixInverse"  : return matrixInverse(args, ret);
                case "copyMatrix"     : return copyMatrix(args, ret);
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
        }

        /// <summary>
        /// 配列変数の存在を確認(内部関数)
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>0:存在しない/1:存在する</returns>
        public Token contains(Token args)
        {
            if (mVariables.ContainsKey(args.mValue))
                return new Token("1", TokenType.LITERAL);
            return new Token("0", TokenType.LITERAL);
        }

        /// <summary>
        /// 配列のサイズの取得(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>サイズ</returns>
        public Token getCount(Token args)
        {
            int cp = args.mValue.LastIndexOf(',');
            int sp = args.mValue.IndexOf("[");
            if (0 < args.mValue.IndexOf("[,]"))
                cp = -1;
            string arrayName = "";
            if (0 < cp)
                arrayName = args.mValue.Substring(0, cp + 1);   //  行単位の配列名(abc[a,
            else if (0 < sp)
                arrayName = args.mValue.Substring(0, sp);       //  配列名(abc[)
            int count = 0;
            foreach (var variable in mVariables) {
                if (0 < cp) {
                    //  2D配列の行単位でカウント
                    int ecp = variable.Key.LastIndexOf(",");
                    if (0 < ecp)
                        if (variable.Key.Substring(0, ecp + 1) == arrayName) count++;
                } else {
                    //  全カウント
                    int esp = variable.Key.IndexOf("[");
                    if (0 <= esp)
                        if (variable.Key.Substring(0, esp) == arrayName) count++;
                }
            }
            return new Token(count.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列をクリア(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        public void clear(Token args)
        {
            string arrayName = args.mValue.Substring(0, args.mValue.IndexOf("["));
            int count = 0;
            foreach (var variable in mVariables) {
                int sp = variable.Key.IndexOf("[");
                if (0 <= sp) {
                    if (variable.Key.Substring(0, sp) == arrayName)
                        mVariables.Remove(variable.Key);
                }
            }
        }

        /// <summary>
        /// 配列から要素を削除する(remove(a[],st[,ed]);)
        /// </summary>
        /// <param name="arg">配列名と要素番号</param>
        public void remove(Token arg)
        {
            List<string> args = mLexer.commaSplit(arg.mValue);
            if (args.Count < 2) return;
            (string arrayName, int no) = getArrayName(new Token(args[0], TokenType.VARIABLE));
            int st = ylib.intParse(args[1]);
            int ed = args.Count > 2 ? ylib.intParse(args[2]) : st;
            for (int i = st; i <= ed; i++) {
                string key = $"{arrayName}[{i}]";
                if (mVariables.ContainsKey(key))
                    mVariables.Remove(key);
            }
            squeeze(new Token(args[0], TokenType.VARIABLE));
        }

        /// <summary>
        /// 配列の圧縮(未使用インデックス削除))
        /// </summary>
        /// <param name="arg">配列名</param>
        public void squeeze(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            if (no != 1)
                return;
            List<Token> listToken = new();
            int maxcol = getMaxArray(arrayName);
            for (int i =0; i < maxcol + 1; i++) {
                string key = $"{arrayName}[{i}]";
                if (mVariables.ContainsKey(key)) {
                    if (mVariables[key] != null)
                        listToken.Add(mVariables[key]);
                    mVariables.Remove(key);
                }
            }
            for (int i = 0; i < listToken.Count; i++) {
                string key = $"{arrayName}[{i}]";
                mVariables.Add(key, listToken[i]);
            }
        }

        /// <summary>
        /// ソート
        /// </summary>
        /// <param name="arg">配列名</param>
        public void sort(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            if (no != 1)
                return ;
            if (isStringArray(arg)) {
                //  文字列のソート
                string[]? strArray = cnvArrayString(arg);
                Array.Sort(strArray);
                clear(arg);
                setReturnArray(strArray, arg);
            } else {
                //  実数のソート
                double[]? doubleArray = cnvArrayDouble(arg);
                Array.Sort(doubleArray);
                clear(arg);
                setReturnArray(doubleArray, arg);
            }
        }

        /// <summary>
        /// ソート
        /// </summary>
        /// <param name="arg">1D配列名</param>
        /// <param name="ret">配列の戻り値名</param>
        /// <returns></returns>
        //public Token sort(Token arg, Token ret)
        //{
        //    if (isStringArray(arg)) {
        //        //  文字列のソート
        //        string[]? strArray = cnvArrayString(arg);
        //        Array.Sort(strArray);
        //        setReturnArray(strArray, ret);
        //    } else {
        //        //  実数のソート
        //        double[]? doubleArray = cnvArrayDouble(arg);
        //        Array.Sort(doubleArray);
        //        setReturnArray(doubleArray, ret);
        //    }
        //    //  戻り値の設定
        //    mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
        //    return mScript.mParse.mVariables["return"];
        //}

        /// <summary>
        /// 配列を逆順にする
        /// </summary>
        /// <param name="arg">配列名</param>
        public void reverse(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            if (no != 1) return ;
            int maxcol = getMaxArray(arrayName);
            if (maxcol <= 0) return ;
            Token[] tokens = new Token[maxcol + 1];
            arrayName += "[";
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
                    (string name, int col) = getArrayNo(variable.Key);
                    tokens[maxcol - col] = variable.Value;
                }
            }
            clear(arg);
            setReturnArray(tokens, arg);
        }

        /// <summary>
        /// 最大値を求める(a[], a[,], a[x,])
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>最大値</returns>
        public Token max(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            double max = double.MinValue;
            if (no == 1 || no == 2) {
                arrayName = getSearchName(arg);
            } else
                return new Token(arrayName, TokenType.ERROR);
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType != TokenType.STRING) {
                        double x = ylib.doubleParse(variable.Value.mValue);
                        if (max < x)
                            max = x;
                    }
                }
            }
            return new Token(max.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 最小値を求める(a[], a[,], a[x,])
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>最小値</returns>
        public Token min(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            double min = double.MaxValue;
            if (no == 1 || no == 2) {
                arrayName = getSearchName(arg);
            } else
                return new Token(arrayName, TokenType.ERROR);
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType != TokenType.STRING) {
                        double x = ylib.doubleParse(variable.Value.mValue);
                        if (min > x)
                            min = x;
                    }
                }
            }
            return new Token(min.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列の合計
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>合計</returns>
        public Token sum(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = cnvListDouble(arg);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double sum = listData.Sum();
            return new Token(sum.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 平均値を求める
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>平均値</returns>
        public Token average(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = cnvListDouble(arg);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double ave = listData.Sum() / listData.Count;
            return new Token(ave.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 分散
        /// </summary>
        /// <param name="arg">配列</param>
        /// <returns>分散値</returns>
        public Token variance(Token arg)
        {
            (string arrayName, int no) = getArrayName(arg);
            List<double> listData = new();
            if (no == 1 || no == 2) {
                listData = cnvListDouble(arg);
            } else
                return new Token(arrayName, TokenType.ERROR);
            double ave = listData.Sum() / listData.Count;
            double vari = listData.Sum(p => (p - ave) * (p - ave)) / listData.Count;
            return new Token(vari.ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 標準偏差
        /// </summary>
        /// <param name="arg">配列</param>
        /// <returns>標準偏差</returns>
        public Token standardDeviation(Token arg)
        {
            Token token = variance(arg);
            if (token.mType != TokenType.ERROR)
                return new Token(Math.Sqrt(ylib.doubleParse(token.mValue)).ToString(), TokenType.LITERAL);
            else
                return token;
        }

        /// <summary>
        /// 共分散(a[],b[])
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>共分散</returns>
        public Token covariance(Token args)
        {
            List<string> listArg = mLexer.commaSplit(args.mValue);
            if (listArg.Count < 2)
                return new Token("", TokenType.ERROR);
            List<double> listData0 = cnvListDouble(new Token(listArg[0], TokenType.ARRAY));
            List<double> listData1 = cnvListDouble(new Token(listArg[1], TokenType.ARRAY));
            if (listData0.Count != listData1.Count)
                return new Token("", TokenType.ERROR);
            double ave0 = listData0.Average();
            double ave1 = listData1.Average();
            double total = 0;
            for (int i = 0; i < listData0.Count; i++) {
                total += (listData0[i] - ave0) * (listData1[i] - ave1);
            }
            return new Token((total / listData0.Count).ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 相関係数(x[],y[])
        /// </summary>
        /// <param name="args">配列</param>
        /// <returns>相関係数</returns>
        public Token correlationCoefficient(Token args)
        {
            List<string> listArg = mLexer.commaSplit(args.mValue);
            if (listArg.Count < 2)
                return new Token("", TokenType.ERROR);
            List<double> x = cnvListDouble(new Token(listArg[0], TokenType.ARRAY));
            List<double> y = cnvListDouble(new Token(listArg[1], TokenType.ARRAY));
            if (x.Count != y.Count)
                return new Token("", TokenType.ERROR);
            double avex = x.Average();
            double avey = y.Average();
            double cov = 0;
            for (int i = 0; i < x.Count; i++) {
                cov += (x[i] - avex) * (y[i] - avey);
            }
            cov /= x.Count;
            double stdx = Math.Sqrt(x.Sum(p => (p - avex) * (p - avex)) / x.Count);
            double stdy = Math.Sqrt(y.Sum(p => (p - avey) * (p - avey)) / y.Count);
            return new Token((cov / (stdx * stdy)).ToString(), TokenType.LITERAL);
        }

        /// <summary>
        /// 配列データを実数のリストに変換
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>実数リスト</returns>
        private List<double> cnvListDouble(Token arg)
        {
            List<double> listData = new List<double>();
            string arrayName = getSearchName(arg);
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf(arrayName)) {
                    if (variable.Value.mType != TokenType.STRING)
                        listData.Add(ylib.doubleParse(variable.Value.mValue));
                }
            }
            return listData;
        }

        /// <summary>
        /// 配列検索用の配列名を求める(arrayName[, arraName[, , arrayName[aa, )
        /// </summary>
        /// <param name="arg">配列名</param>
        /// <returns>検索用配列名</returns>
        private string getSearchName(Token arg)
        {
            string arrayName = "";
            if (0 <= arg.mValue.IndexOf("[]"))
                arrayName = arg.mValue.Substring(0, arg.mValue.IndexOf('[') + 1);
            else if (0 <= arg.mValue.IndexOf("[,]"))
                arrayName = arg.mValue.Substring(0, arg.mValue.IndexOf('[') + 1);
            else if (0 <= arg.mValue.IndexOf(",]"))
                arrayName = arg.mValue.Substring(0, arg.mValue.IndexOf(',') + 1);
            else
                arrayName = arg.mValue;
            return arrayName;
        }

        /// <summary>
        /// Windowsコマンド実行
        /// </summary>
        /// <param name="args"></param>
        private void cmd(Token args)
        {
            ylib.openUrl(args.mValue);
        }

        /// <summary>
        /// 単位行列の作成(n x n)
        /// </summary>
        /// <param name="size">行列の大きさ</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token unitMatrix(Token size, Token ret)
        {
            double[,] matrix = ylib.unitMatrix(ylib.intParse(size.mValue));

            //  戻り値の設定
            setReturnArray(matrix, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 転置行列  行列Aの転置A^T
        /// </summary>
        /// <param name="args">引数(行列 A</param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private Token matrixTranspose(Token args, Token ret)
        {
            //  2D配列を実数配列に変換
            List<string> listArg = mLexer.commaSplit(args.mValue);
            double[,]? a = cnvArrayDouble2(new Token(listArg[0], TokenType.ARRAY));
            //  行列演算
            double[,] c = ylib.matrixTranspose(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 行列の積  AxB
        /// 行列の積では 結合の法則  (AxB)xC = Ax(BxC) , 分配の法則 (A+B)xC = AxC+BxC , Cx(A+B) = CxA + CxB　が可
        /// 交換の法則は成立しない  AxB ≠ BxA
        /// </summary>
        /// <param name="args">引数(行列A,行列B)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixMulti(Token args, Token ret)
        {
            //  2D配列を実数配列に変換
            List<string> listArg = mLexer.commaSplit(args.mValue);
            double[,]? a = cnvArrayDouble2(new Token(listArg[0], TokenType.ARRAY));
            double[,]? b = cnvArrayDouble2(new Token(listArg[1], TokenType.ARRAY));
            //  行列演算
            double[,] c = ylib.matrixMulti(a, b);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 行列の和 A+B
        /// 異なるサイズの行列はゼロ行列にする
        /// </summary>
        /// <param name="args">引数(行列A,行列B)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixAdd(Token args, Token ret)
        {
            //  2D配列を実数配列に変換
            List<string> listArg = mLexer.commaSplit(args.mValue);
            double[,]? a = cnvArrayDouble2(new Token(listArg[0], TokenType.ARRAY));
            double[,]? b = cnvArrayDouble2(new Token(listArg[1], TokenType.ARRAY));
            //  行列演算
            double[,] c = ylib.matrixAdd(a, b);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 逆行列 A^-1 (ある行列で線形変換した空間を元に戻す行列)
        /// </summary>
        /// <param name="args">引数(行列A)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token matrixInverse(Token args, Token ret)
        {
            //  2D配列を実数配列に変換
            List<string> listArg = mLexer.commaSplit(args.mValue);
            double[,]? a = cnvArrayDouble2(new Token(listArg[0], TokenType.ARRAY));
            //  行列演算
            double[,] c = ylib.matrixInverse(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 行列のコピー
        /// </summary>
        /// <param name="args">引数(行列A)</param>
        /// <param name="ret">戻り変数</param>
        /// <returns>戻り変数</returns>
        private Token copyMatrix(Token args, Token ret)
        {
            //  2D配列を実数配列に変換
            List<string> listArg = mLexer.commaSplit(args.mValue);
            double[,]? a = cnvArrayDouble2(new Token(listArg[0], TokenType.ARRAY));
            //  行列演算
            double[,] c = ylib.copyMatrix(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 配列に文字列名があるかの確認
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>文字列あるなし</returns>
        public bool isStringArray(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (0 < no) {
                foreach (var variable in mVariables) {
                    if (variable.Key.IndexOf($"{arrayName}[") == 0) {
                        Token token = mVariables[variable.Key];
                        if (token.mType == TokenType.STRING)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 配列をstring[]に変換
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>string[]</returns>
        public string[]? cnvArrayString(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 1)
                return null;
            int maxCol = getMaxArray(arrayName);
            string[] ret = new string[maxCol + 1];
            for (int j = 0; j <= maxCol; j++) {
                string name = $"{arrayName}[{j}]";
                if (mVariables.ContainsKey(name))
                    ret[j] = mVariables[name].mValue;
            }
            return ret;
        }

        /// <summary>
        /// 配列をdouble[]に変換
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>double[]</returns>
        public double[]? cnvArrayDouble(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 1)
                return null;
            int maxCol = getMaxArray(arrayName);
            double[] ret = new double[maxCol + 1];
            for (int j = 0; j <= maxCol; j++) {
                string name = $"{arrayName}[{j}]";
                if (mVariables.ContainsKey(name))
                    ret[j] = ylib.doubleParse(mVariables[name].mValue);
            }
            return ret;
        }

        /// <summary>
        /// 配列の最大インデックスを求める
        /// </summary>
        /// <param name="arrayName">配列名</param>
        /// <returns>最大インデックス値</returns>
        public int getMaxArray(string arrayName)
        {
            int maxCol = 0;
            foreach (var variable in mVariables) {
                (string name, int? col) = getArrayNo(variable.Key);
                if (name == arrayName && col != null)
                    maxCol = Math.Max(maxCol, (int)col);
            }
            return maxCol;
        }

        /// <summary>
        /// 配列変数を実数配列double[,]に変換
        /// </summary>
        /// <param name="args">配列変数</param>
        /// <returns>実数配列</returns>
        public double[,]? cnvArrayDouble2(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            if (no != 2)
                return null;
            int maxRow = 0, maxCol = 0;
            foreach (var variable in mVariables) {
                (string name, int? row, int? col) = getArrayNo2(variable.Key);
                if (name == arrayName && row != null && col != null) {
                    maxRow = Math.Max(maxRow, (int)row);
                    maxCol = Math.Max(maxCol, (int)col);
                }
            }
            double[,] ret = new double[maxRow + 1, maxCol + 1];
            for (int i = 0; i <= maxRow; i++) {
                for (int j = 0; j <= maxCol; j++) {
                    string name = $"{arrayName}[{i},{j}]";
                    if (mVariables.ContainsKey(name))
                        ret[i, j] = ylib.doubleParse(mVariables[name].mValue);
                }
            }
            return ret;
        }

        /// <summary>
        /// 配列戻り値に設定
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(Token[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                mScript.mParse.addVariable(key, src[i].copy());
            }
        }

        /// <summary>
        /// 2D配列の戻り値に設定
        /// </summary>
        /// <param name="src">2D配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(double[,] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[,]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.GetLength(0); i++) {
                for (int j = 0; j < src.GetLength(1); j++) {
                    Token key = new Token($"{destName}[{i},{j}]", TokenType.VARIABLE);
                    mScript.mParse.addVariable(key, new Token(src[i, j].ToString(), TokenType.LITERAL));
                }
            }
        }

        /// <summary>
        /// 配列の戻り値に設定
        /// </summary>
        /// <param name="src">配列データ</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(double[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                mScript.mParse.addVariable(key, new Token(src[i].ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 文字列配列を戻り値に設定
        /// </summary>
        /// <param name="src">文字列配列</param>
        /// <param name="dest">戻り値の配列名</param>
        private void setReturnArray(string[] src, Token dest)
        {
            if (src == null || dest == null) return;
            int dp = dest.mValue.IndexOf("[]");
            if (dp < 0) return;
            string destName = dest.mValue.Substring(0, dp);
            for (int i = 0; i < src.Length; i++) {
                Token key = new Token($"{destName}[{i}]", TokenType.VARIABLE);
                mScript.mParse.addVariable(key, new Token(src[i].ToString(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 配列から配列名と配列のインデックスを取得
        /// </summary>
        /// <param name="arrayName">配列</param>
        /// <returns>(配列名,インデックス)</returns>
        public (string name, int index) getArrayNo(string arrayName)
        {
            List<Token> splitName = mLexer.splitArgList(arrayName);
            if (splitName.Count < 2)
                return ("", -1);
            string name = splitName[0].mValue;
            int index = ylib.intParse(splitName[2].mValue);
            return (name, index);
        }

        /// <summary>
        /// 2次元配列から配列名と行と列を取り出す
        /// </summary>
        /// <param name="arrayName">2D配列</param>
        /// <returns>(配列名、行、列)</returns>
        public (string name, int? row, int? col) getArrayNo2(string arrayName)
        {
            List<Token> splitName = mLexer.splitArgList(arrayName);
            if (splitName.Count < 3)
                return ("", null, null);
            string name = splitName[0].mValue;
            int row = ylib.intParse(splitName[2].mValue);
            int col = ylib.intParse(splitName[4].mValue);
            return (name, row, col);
        }

        /// <summary>
        /// 変数名または配列名と配列の次元の取得
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>(配列名, 次元)</returns>
        public (string name, int no) getArrayName(Token args)
        {
            int dimNo = 0;
            int cp = args.mValue.LastIndexOf(',');
            int sp = args.mValue.IndexOf("[");
            if (0 < sp && cp < 0) dimNo = 1;
            if (0 < sp && 0 < cp) dimNo = 2;
            if (0 < args.mValue.IndexOf("[,]"))
                cp = -1;
            string arrayName = "";
            if (0 < cp)
                arrayName = args.mValue.Substring(0, cp + 1);
            else if (0 < sp)
                arrayName = args.mValue.Substring(0, sp);
            return (arrayName, dimNo);
        }

        /// <summary>
        /// 引数の変数または配列変数を数値に変換
        /// m + 2 →  3 + 2 → 5
        /// a[m, n+1] → a[2,3+1] → a[2,4] → 5
        /// </summary>
        /// <param name="value">変数または配列変数</param>
        /// <returns>数値</returns>
        public Token getValueToken(string value)
        {
            string buf = "";
            int sp = value.IndexOf("[");
            if (0 <= sp) {
                //  配列引数
                List<Token> tokens = mLexer.splitArgList(value);
                buf = tokens[0].mValue;
                for (int i = 1; i < tokens.Count; i++) {
                    if (tokens[i].mType == TokenType.VARIABLE ||
                        tokens[i].mType == TokenType.EXPRESS) {
                        buf += mScript.express(tokens[i]).mValue;
                    } else {
                        buf += tokens[i].mValue;
                    }
                }
            } else {
                //  通常の引数
                buf = mScript.express(new Token(value, TokenType.VARIABLE)).mValue;
            }

            if (mVariables.ContainsKey(buf))
                return mVariables[buf];
            else
                return new Token(buf, TokenType.LITERAL);
        }
    }
}
