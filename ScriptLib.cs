using CoreLib;

namespace KScript
{
    /// <summary>
    /// 追加内部関数
    ///     input : a = input();                        キー入力(文字列)
    ///     arrayContain : a = arrayContain(c[2]);      配列の有無(0:なし 1:あり)
    ///     arraySize :　size = arraySize(a[]);         1次元配列のサイズ
    ///                  size = arraySize(b[,]);        2次元配列のサイズ
    ///                  size = arraySize(b[2,]);       2次元配列1列目のサイズ
    ///     arrayClear : arrayClear(a[]);               配列クリア
    ///     cmd : cmd(command);                             Windowsコマンドの実行
    ///     
    /// 追加したい関数
    ///     load : text = load(path);                       テキストファイルの読込み
    ///     save : save(path, text);                        テキストファイルの書き込み
    ///     unitMatrix : a[,] = unitMatrix(size);           単位行列の作成
    ///     matrixTranspose : b[,] = matrixTranspose(a[,]); 転置行列  行列Aの転置A^T
    ///     matrixMulti : c[,] = matrixMulti(a[,], b[,]);   行列の積 AxB
    ///     matrixAdd : c[,] = matrixAdd(a[,], b[,]);       行列の和 A+B
    ///     matrixInverse : b[,] = matrixInverse(a[,]);     逆行列 A^-1
    ///     copyMatrix : b[,] = copyMatrix(a[,]);           行列のコピー
    ///     graph                                           グラフ表示
    ///     chart                                           チャート表示
    ///     table                                           表編集
    ///     imageView                                       イメージ表示
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
                case "input": return new Token(Console.ReadLine(), TokenType.STRING);
                case "arrayContain": return new Token(arrayContain(args).ToString(), TokenType.LITERAL);
                case "arraySize": return new Token(getArraySize(args).ToString(), TokenType.LITERAL);
                case "arrayClear": arrayClear(args); break;
                case "cmd": cmd(args); break;
                case "unitMatrix": return unitMatrix(args, ret);
                case "matrixTranspose": return matrixTranspose(args, ret);
                case "matrixMulti": return matrixMulti(args, ret);
                case "matrixAdd": return matrixAdd(args, ret);
                case "matrixInverse": return matrixInverse(args, ret);
                case "copyMatrix": return copyMatrix(args, ret);
                default: return new Token("not found func", TokenType.ERROR);
            }
            return new Token("", TokenType.EMPTY);
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
            double[,] a = cnvArrayDouble(new Token(listArg[0], TokenType.ARRAY));
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
            double[,] a = cnvArrayDouble(new Token(listArg[0], TokenType.ARRAY));
            double[,] b = cnvArrayDouble(new Token(listArg[1], TokenType.ARRAY));
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
            double[,] a = cnvArrayDouble(new Token(listArg[0], TokenType.ARRAY));
            double[,] b = cnvArrayDouble(new Token(listArg[1], TokenType.ARRAY));
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
            double[,] a = cnvArrayDouble(new Token(listArg[0], TokenType.ARRAY));
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
            double[,] a = cnvArrayDouble(new Token(listArg[0], TokenType.ARRAY));
            //  行列演算
            double[,] c = ylib.copyMatrix(a);
            //  戻り値の設定
            setReturnArray(c, ret);
            mScript.mParse.addVariable(new Token("return", TokenType.VARIABLE), ret);
            return mScript.mParse.mVariables["return"];
        }

        /// <summary>
        /// 配列の戻り値の設定
        /// </summary>
        /// <param name="src">2D配列データ</param>
        /// <param name="dest">戻り値の配列変数名</param>
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
        /// Windowsコマンド実行
        /// </summary>
        /// <param name="args"></param>
        private void cmd(Token args)
        {
            ylib.openUrl(args.mValue);
        }

        /// <summary>
        /// 配列変数の存在を確認(内部関数)
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>0:存在しない/1:存在する</returns>
        public int arrayContain(Token args)
        {
            if (mVariables.ContainsKey(args.mValue))
                return 1;
            return 0;
        }

        /// <summary>
        /// 配列のサイズの取得(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>サイズ</returns>
        public int getArraySize(Token args)
        {
            int cp = args.mValue.LastIndexOf(',');
            int sp = args.mValue.IndexOf("[");
            if (0 < args.mValue.IndexOf("[,]"))
                cp = -1;
            string arrayName = "";
            if (0 < cp)
                arrayName = args.mValue.Substring(0, cp + 1);
            else if (0 < sp)
                arrayName = args.mValue.Substring(0, sp);
            int count = 0;
            foreach (var variable in mVariables) {
                if (0 < cp) {
                    int ecp = variable.Key.LastIndexOf(",");
                    if (0 < ecp)
                        if (variable.Key.Substring(0, ecp + 1) == arrayName) count++;
                } else {
                    int esp = variable.Key.IndexOf("[");
                    if (0 <= esp)
                        if (variable.Key.Substring(0, esp) == arrayName) count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 配列をクリア(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        public void arrayClear(Token args)
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
        /// 配列変数を実数配列double[,]に変換
        /// </summary>
        /// <param name="args">配列変数</param>
        /// <returns>実数配列</returns>
        public double[,] cnvArrayDouble(Token args)
        {
            (string arrayName, int no) = getArrayName(args);
            int maxRow = 0, maxCol = 0;
            foreach (var variable in mVariables) {
                (string name, int? row, int? col) = getArrayNo(variable.Key);
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
        /// 2次元配列から配列名と行と列を取り出す
        /// </summary>
        /// <param name="arrayName">2D配列</param>
        /// <returns>(配列名、行、列)</returns>
        public (string name, int? row, int? col) getArrayNo(string arrayName)
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
