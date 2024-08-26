﻿using CoreLib;

namespace KScript
{
    /// <summary>
    /// スクリプトの処理
    ///     
    /// 
    /// 関数の構成
    ///   Script            スクリプトの読み込み登録
    ///     execute             スクリプト文の逐次処理
    ///       exeStatements         複数構文の実行
    ///         exeStatement            構文の実行
    ///         　　letStatement              代入文の処理
    ///         　　  funcStatement
    ///         　　  express
    ///         　　  setArrayData
    ///         　　  getArrayVariable
    ///         　　ifStatement               if文の処理
    ///         　　  conditinalStatement
    ///         　　  exeStatements
    ///         　　whileStatement            while文の処理
    ///         　　  conditinalStatement
    ///         　　  exeStatements
    ///         　　forStatement              for文の処理
    ///         　　  mParse.getStatements
    ///         　　  letStatement
    ///         　　  conditinalStatement
    ///         　　  exeStatements
    ///         　　returnStatement           return文の処理
    ///         　　  express
    ///         　　printStatement            print文の処理
    ///         　　  express
    ///         　　funcStatement             関数文の実行
    ///         　　  funcExpress
    ///                 
    ///   funcExpress               関数処理
    ///     innerFunc                   追加内部関数
    ///     programFunc                 プログラム関数
    ///     funcExpress                 数式処理関数
    /// 
    /// 関数一覧
    /// Script(string script)                                   コンストラクタ(スクリプト文の設定)
    /// execute(string funcName, List<Token> arg = null)        スクリプトの実行
    /// exeStatements(List<Token> tokens, int sp)               複数の文の処理
    /// exeStatement(List<Token> tokens)                        文の処理
    /// letStatement(List<Token> tokens)                        代入文の処理
    /// ifStatement(List<Token> tokens)                         if文の処理
    /// whileStatement(List<Token> tokens)                      while文の処理
    /// forStatement(List<Token> tokens)                        for文の処理
    /// printStatement(List<Token> tokens)                      print文の処理
    /// returnStatement(List<Token> tokens)                     return文の処理(return 変数に登録)
    /// funcStatement(List<Token> tokens, int sp, Token ret = null) 関数文の実行
    /// funcExpress(Token funcName, Token arg, Token ret = null)    関数処理
    /// 
    /// programFunc(string funcName, Token arg, Token ret)      プログラム関数の実行(別 Script で実行)
    /// innerFunc(Token funcName, Token arg)                    追加内部関数
    /// arrayContain(Token args)                                配列変数の存在を確認(内部関数)
    /// getArraySize(Token args)                                配列のサイズの取得(内部関数)
    /// arrayClear(Token args)                                  配列をクリア(内部関数)
    /// 
    /// conditinalStatement(List<Token> tokens, int sp = 0)     条件文の処理(比較) (a < b, a == b...)
    /// 
    /// express(Token token)                                    数式処理
    /// express(List<Token> tokens, int sp = 0)                 数式処理(文字列を含むときは文字列に変換して結合)
    /// funcExpress(string funcName, Token arg)                 数式関数の処理
    /// 
    /// getFuncArgs(string func, int sp = 0)                    プログラム関数引数をリストに変換
    /// setFuncArg(List<Token> src, List<Token> dest, Script script)    プログラム関数の引数を関数側にコピー
    /// cnvArgData(Token arg)                                   引数の数式を変換した引数にする
    /// cnvArgList(Token arg)                                   引数をリストに変換( '(a,b,c)' →  'a','b','c' )
    /// getVariable(Token token)                                変数名を数値に変換
    /// getValueToken(string value)                             変数または配列変数を数値に変換
    /// getArrayVariable(Token token)                           配列変数名の変換([m,n] → [2,3])
    /// setArrayData(List<Token> tokens)                        配列の一括値設定(a[] = { 1,2,1,3} )
    /// setArrayData(Token name, Token data)                    1次元配列の一括値設定(a[] = { 1,2,1,3} )
    /// setArrayData2(Token name, Token data)                   2次元配列の一括値設定(a[,] = { { 1,2,1,3}, {2,3,4,5} } )
    /// getFuncArray(Token src, Token dest, Script parse)       プログラム関数の戻り値受け渡し
    /// setFuncArray(Token src, Token dest, Script script)      プログラム関数の引数受け渡し
    /// 
    /// printToken(string title, List<Token> tokens, bool type = false, bool debug = false, bool console = false)
    /// tokensString(List<Token> tokens)                        デバッグ用トークンリストの文字列か
    /// 
    /// </summary>
    public class Script
    {
        public List<Token> mTokenList = new List<Token>();
        public List<List<Token>> mStatements = new List<List<Token>>();

        private KParse mParse = new KParse();
        private KLexer mLexer = new KLexer();
        private YCalc mCalc = new YCalc();

        /// <summary>
        /// コンストラクタ(スクリプト文の設定)
        /// </summary>
        /// <param name="script">スクリプト文</param>
        public Script(string script)
        {
            //  字句解析・ スクリプト登録(mFunctionsに登録)
            mTokenList = mLexer.tokenList(script);
            mStatements = mParse.getStatements(mTokenList);
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        public void execute(string funcName, List<Token> arg = null)
        {
            //  スクリプト関数処理部の抽出・登録
            List<List<Token>> funcStatement = null;
            if (mParse.mFunctions.ContainsKey(funcName)) {
                //  関数指定
                Token func = mParse.mFunctions[funcName];
                List<Token> funcList = mParse.getStatement(mLexer.tokenList(func.mValue));
                funcStatement = mParse.getStatements(mLexer.tokenList(mLexer.stripBracketString(funcList[2].mValue, funcList[2].mValue[0])));
                //  引数の登録
                if (arg != null) {
                    string[] funcargs = mLexer.stripBracketString(funcList[1].mValue, '(').Split(',');
                    for (int i = 0; i < funcargs.Length; i++)
                        mParse.addVariable(new Token(funcargs[i].Trim(), TokenType.VARIABLE), arg[i]);
                }
            }
            //  構文実行
            if (funcStatement == null)
                funcStatement = mStatements;
            foreach (List<Token> statement in funcStatement) {
                if (exeStatement(statement) != RETURNTYPE.NORMAL)
                    break;
            }
        }

        /// <summary>
        /// 複数の文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>実行結果</returns>
        private RETURNTYPE exeStatements(List<Token> tokens, int sp)
        {
            RETURNTYPE returnType = RETURNTYPE.NORMAL;
            List<Token> tokenList = new List<Token>();
            if (tokens[sp].mType == TokenType.STATEMENTS)
                tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[sp].mValue, tokens[sp].mValue[0]));
            else
                tokenList.AddRange(tokens.Skip(sp));
            List<List<Token>> statements = mParse.getStatements(tokenList);
            foreach (var statement in statements) {
                returnType = exeStatement(statement);
                if (returnType != RETURNTYPE.NORMAL)
                    break;
            }
            return returnType;
        }

        /// <summary>
        /// 文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE exeStatement(List<Token> tokens)
        {
            //printToken("", tokens, true, true, false);
            if (tokens[0].mType == TokenType.VARIABLE ||
                tokens[0].mType == TokenType.ARRAY) {
                letStatement(tokens);
            } else if (tokens[0].mType == TokenType.STATEMENT) {
                if (tokens[0].mValue == "print") {
                    printStatement(tokens);
                } else if (tokens[0].mValue == "if") {
                    return ifStatement(tokens);
                } else if (tokens[0].mValue == "while") {
                    return whileStatement(tokens);
                } else if (tokens[0].mValue == "for") {
                    return forStatement(tokens);
                } else if (tokens[0].mValue == "return") {
                    returnStatement(tokens);
                    return RETURNTYPE.RETURN;
                } else if (tokens[0].mValue == "break") {
                    return RETURNTYPE.BREAK;
                } else if (tokens[0].mValue == "continue") {
                    return RETURNTYPE.CONTINUE;
                } else {
                    System.Diagnostics.Debug.WriteLine($"exeStatement: {tokens} ");
                }
            } else if (tokens[0].mType == TokenType.FUNCTION) {
                funcStatement(tokens, 0, null);
            } else if (tokens[0].mType == TokenType.COMMENT) {
                System.Diagnostics.Debug.WriteLine($"Comment: {tokens[0].mValue} ");
            } else {
                System.Diagnostics.Debug.WriteLine($"exeStatement: {tokens} ");
            }
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// 代入文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public void letStatement(List<Token> tokens)
        {
            Token variable = null;
            List<Token> expressList = new List<Token>();
            if (2 <= tokens.Count) {
                if (0 <= tokens[0].mValue.IndexOf("[]") ||
                        0 <= tokens[0].mValue.IndexOf(",]")) {
                    //  配列一括設定
                    if (tokens[2].mType == TokenType.STATEMENTS) {
                        setArrayData(tokens);
                    } else if (tokens[2].mType == TokenType.FUNCTION) {
                        funcStatement(tokens, 2, tokens[0]);
                    } else {
                        Console.WriteLine($"Error: {tokensString(tokens)}");
                    }
                    return;
                } else if (tokens[0].mType == TokenType.VARIABLE ||
                    tokens[0].mType == TokenType.ARRAY) {
                    //  変数、配列に代入
                    variable = getArrayVariable(tokens[0]);
                    expressList.AddRange(tokens.Skip(2));
                } else {
                    Console.WriteLine($"Error: {tokensString(tokens)}");
                    return;
                }
                if (tokens[1].mType == TokenType.ASSIGNMENT) {
                    //  複合演算子(++.--,+=,-=,*=,/=,^=)
                    //  [変数 = 変数 [+,-,*,/,^] 値] の形式に変換
                    Token token = new Token("1", TokenType.LITERAL);    //  '++','--'の時値
                    if (tokens[1].mValue.Length == 2) {
                        //  '++','--' 以外の複合演算子
                        if (tokens[1].mValue[1] == '=')
                            token = express(tokens, 2);
                        expressList = new List<Token>(){
                            tokens[0], 
                            new Token(tokens[1].mValue[0].ToString(), TokenType.OPERATOR),
                            token
                        };
                    }
                } else {
                    Console.WriteLine($"Error: {tokensString(tokens)}");
                    return;
                }
                Token value = express(expressList);
                mParse.addVariable(variable, value);
            }
        }

        /// <summary>
        /// if文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE ifStatement(List<Token> tokens)
        {
            if (conditinalStatement(tokens, 1)) {
                List<Token> tokenList = mParse.getStatement(tokens, 2);
                return exeStatements(tokenList, 0);
            } else {
                int n = tokens.FindIndex(p => p.mValue == "else");
                if (0 < n) {
                    List<Token> tokenList = mParse.getStatement(tokens, n + 1);
                    return exeStatements(tokenList, 0);
                }
            }
            //calcError("ifStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// while文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE whileStatement(List<Token> tokens)
        {
            while (conditinalStatement(tokens, 1)) {
                RETURNTYPE returnType = exeStatements(tokens, 2);
                if (returnType == RETURNTYPE.BREAK)
                    break;
                else if (returnType == RETURNTYPE.RETURN)
                    return RETURNTYPE.RETURN;
            }
            //calcError("whileStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// for文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>実行結果</returns>
        public RETURNTYPE forStatement(List<Token> tokens)
        {
            List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[1].mValue));
            List<List<Token>> funcStatement = mParse.getStatements(tokenList);
            if (funcStatement.Count == 3) {
                letStatement(funcStatement[0]);                     //  初期値
                while (conditinalStatement(funcStatement[1])) {     //  条件
                    RETURNTYPE returnType = exeStatements(tokens, 2);
                    if (returnType == RETURNTYPE.BREAK)
                        break;
                    else if (returnType == RETURNTYPE.RETURN)
                        return RETURNTYPE.RETURN;
                    letStatement(funcStatement[2]);                 //  更新処理
                }
            }
            //calcError("forStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// print文の処理
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public void printStatement(List<Token> tokens)
        {
            if (tokens.Count <= 1) {
                //  データなし改行のみ
                Console.WriteLine();
            } else if (tokens[1].mType == TokenType.EXPRESS) {
                List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[1].mValue));
                if (tokenList == null || tokenList.Count == 0) {
                    //  データなし改行のみ
                    Console.WriteLine();
                } else {
                    //  コンマ区切りのデータ表示
                    List<Token> expList = new List<Token>();
                    for (int i = 0; i < tokenList.Count; i++) {
                        if (tokenList[i].mType == TokenType.DELIMITER) {
                            Console.Write(express(expList).mValue);
                            expList = new List<Token>();
                        } else if (i == tokenList.Count - 1) {
                            expList.Add(tokenList[i]);
                            Console.Write(express(expList).mValue);
                        } else {
                            expList.Add(tokenList[i]);
                        }
                    }
                }
            } else if (tokens[1].mType == TokenType.STRING) {
                string buf = tokens[1].mValue.Replace("\\n", "\n");
                Console.Write(buf);
            } else {
                //  Error
                Console.WriteLine($"Error: {tokens[1]}");
            }
        }

        /// <summary>
        /// return文の処理(return 変数に登録)
        /// </summary>
        /// <param name="tokens">トークカリスト</param>
        public void returnStatement(List<Token> tokens)
        {
            Token token;
            if (0 <= tokens[1].mValue.IndexOf("[]") || 0 <= tokens[1].mValue.IndexOf("[,]"))
                token = tokens[1].copy();
            else
               token = express(tokens, 1);
            mParse.addVariable(new Token("return", TokenType.VARIABLE), token);
        }

        /// <summary>
        /// 関数文の実行
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <param name="ret">配列の戻り値</param>
        /// <returns>戻り値</returns>
        public void funcStatement(List<Token> tokens, int sp, Token ret = null)
        {
            Token funcName = tokens[sp];
            Token arg = tokens[sp + 1];
            Token result = funcExpress(funcName, arg, ret);
            if (result != null)
                mParse.addVariable(new Token("return", TokenType.VARIABLE), result);
        }

        /// <summary>
        /// 関数処理
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">戻り値</param>
        /// <returns></returns>
        private Token funcExpress(Token funcName, Token arg, Token ret = null)
        {
            Token result = innerFunc(funcName, arg);   //  内部関数処理
            if (result != null) {
                return result;
            } else if (mParse.mFunctions.ContainsKey(funcName.mValue)) {
                //  プログラムの関数
                return programFunc(funcName.mValue, arg, ret);
            } else {
                //  数式処理の関数
                return funcExpress(funcName.mValue, arg);
            }
        }

        /// <summary>
        /// プログラム関数の実行(別 Script で実行)
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">配列の戻り値</param>
        /// <returns>戻り値</returns>
        private Token programFunc(string funcName, Token arg, Token ret)
        {
            Script script = new Script(mParse.mFunctions[funcName].mValue);
            script.mParse.mFunctions = mParse.mFunctions;
            //  引数の変換
            List<Token> srcArgs = getFuncArgs(arg.mValue);
            List<Token> outArgs = getFuncArgs(mParse.mFunctions[funcName].mValue, 1);
            setFuncArg(srcArgs, outArgs, script);
            script.execute(funcName, null);
            //  戻り値の設定
            if (script.mParse.mVariables.ContainsKey("return")) {
                getFuncArray(script.mParse.mVariables["return"], ret, script);
                return script.mParse.mVariables["return"];
            } else
                return null;
        }

        /// <summary>
        /// 追加内部関数
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>戻り値</returns>
        private Token innerFunc(Token funcName, Token arg)
        {
            string argValue = mLexer.stripBracketString(arg.mValue);
            Token args = cnvArgData(new Token(argValue, arg.mType));
            switch (funcName.mValue) {
                case "input": return new Token(Console.ReadLine(), TokenType.STRING);
                case "arrayContain": return new Token(arrayContain(args).ToString(), TokenType.LITERAL);
                case "arraySize": return new Token(getArraySize(args).ToString(), TokenType.LITERAL);
                case "arrayClear": arrayClear(args); return null;
                default: break;
            }
            return null;
        }

        /// <summary>
        /// 配列変数の存在を確認(内部関数)
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>0:存在しない/1:存在する</returns>
        public int arrayContain(Token args)
        {
            if (mParse.mVariables.ContainsKey(args.mValue))
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
            Token argsName = getValueToken(args.mValue);
            int cp = argsName.mValue.LastIndexOf(',');
            int sp = argsName.mValue.IndexOf("[");
            if (0 < argsName.mValue.IndexOf("[,]"))
                cp = -1;
            string arrayName = "";
            if (0 < cp)
                arrayName = argsName.mValue.Substring(0, cp + 1);
            else if (0 < sp)
                arrayName = argsName.mValue.Substring(0, sp);
            int count = 0;
            foreach (var variable in mParse.mVariables) {
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
            foreach (var variable in mParse.mVariables) {
                int sp = variable.Key.IndexOf("[");
                if (0 <= sp) {
                    if (variable.Key.Substring(0, sp) == arrayName)
                        mParse.mVariables.Remove(variable.Key);
                }
            }
        }

        /// <summary>
        /// 条件文の処理(比較) (a < b, a == b...)
        /// </summary>
        /// <param name="tokens">条件文</param>
        /// <param name="sp">開始位置</param>
        /// <returns>処理結果</returns>
        public bool conditinalStatement(List<Token> tokens, int sp = 0)
        {
            if (2 < tokens.Count && tokens[1].mType != TokenType.CONDITINAL)
                tokens = mLexer.tokenList(mLexer.stripBracketString(tokens[sp].mValue));
            else if (tokens.Count == 1)
                tokens = mLexer.tokenList(mLexer.stripBracketString(tokens[0].mValue));

            List<Token> a = new List<Token>();
            List<Token> b = new List<Token>();
            int n = 0;
            while (n < tokens.Count && tokens[n].mType != TokenType.CONDITINAL)
                a.Add(tokens[n++]);
            if (n == tokens.Count) return false;        //  Error
            Token cond = tokens[n++];
            while (n < tokens.Count && tokens[n].mType != TokenType.DELIMITER)
                b.Add(tokens[n++]);
            switch (cond.mValue) {
                case "||": return conditinalStatement(a) || conditinalStatement(b);
                case "&&": return conditinalStatement(a) && conditinalStatement(b);
                case "!": return !conditinalStatement(b);
            }

            double avalue = double.Parse(express(a, 0).mValue);
            double bvalue = double.Parse(express(b, 0).mValue);
            switch (cond.mValue) {
                case "==": return avalue == bvalue;
                case "!=": return avalue != bvalue;
                case "<": return avalue < bvalue;
                case ">": return avalue > bvalue;
                case "<=": return avalue <= bvalue;
                case ">=": return avalue >= bvalue;
                default:    //  Error
                    break;
            }
            return false;
        }

        /// <summary>
        /// 数式処理
        /// </summary>
        /// <param name="token">トークン</param>
        /// <returns>計算結果</returns>
        public Token express(Token token)
        {
            return express(new List<Token>() { token });
        }

        /// <summary>
        /// 数式処理(文字列を含むときは文字列に変換して結合)
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>計算結果(文字列)</returns>
        private Token express(List<Token> tokens, int sp = 0)
        {
            Token buf = null;
            Token token = null;
            for (int i = sp; i < tokens.Count; i++) {
                if (tokens[i].mType == TokenType.DELIMITER) {
                    break;
                } else if (tokens[i].mType == TokenType.LITERAL) {
                    token = getVariable(tokens[i]);
                } else if (tokens[i].mType == TokenType.VARIABLE) {
                    token = getVariable(tokens[i]);
                } else if (tokens[i].mType == TokenType.ARRAY) {
                    token = getArrayVariable(tokens[i]);
                    if (token != null)
                        token = getVariable(token);
                } else if (tokens[i].mType == TokenType.STRING) {
                    token = tokens[i].copy();
                } else if (tokens[i].mType == TokenType.EXPRESS) {
                    List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[i].mValue));
                    token = express(tokenList);
                } else if (tokens[i].mType == TokenType.FUNCTION) {
                    token = funcExpress(tokens[i], tokens[i + 1]);
                    i++;
                } else if (tokens[i].mType == TokenType.OPERATOR) {
                    token = tokens[i].copy();
                } else {
                    //  ERROR
                    Console.WriteLine($"ERROR: {tokens[i]}");
                    break;
                }
                if (buf == null) {
                    buf = token.copy();
                } else if (buf.mType == TokenType.STRING || token.mType == TokenType.STRING) {
                    buf.mValue += token.mValue;
                    buf.mType = TokenType.STRING;
                } else if (token.mType == TokenType.OPERATOR) {
                    buf.mValue += token.mValue;
                    buf.mType = TokenType.EXPRESS;
                } else {
                    buf.mValue += token.mValue;
                    buf.mValue = mCalc.expression(buf.mValue).ToString();
                    buf.mType = TokenType.LITERAL;
                }
            }
            return buf.copy();
        }

        /// <summary>
        /// 数式関数の処理
        /// </summary>
        /// <param name="funcName">数式関数</param>
        /// <param name="arg">引数</param>
        /// <returns>計算結果</returns>
        private Token funcExpress(string funcName, Token arg)
        {
            List<Token> argValue = cnvArgList(arg);
            string buf = "";
            foreach (Token token in argValue)
                buf += express(token).mValue + ",";
            buf = buf.TrimEnd(',');
            string result = mCalc.expression($"{funcName}({buf})").ToString();
            //calcError(funcName, argValue);
            return new Token(result, TokenType.LITERAL);
        }

        /// <summary>
        /// プログラム関数引数をリストに変換
        /// </summary>
        /// <param name="func">引数文字列</param>
        /// <param name="sp">開始位置</param>
        /// <returns>引数リスト</returns>
        private List<Token> getFuncArgs(string func, int sp = 0)
        {
            List<Token> args = new List<Token>();
            List<Token> funcList = mParse.getStatement(mLexer.tokenList(func));
            string[] funcargs = mLexer.stripBracketString(funcList[sp].mValue, '(').Split(',');
            for (int i = 0; i < funcargs.Length; i++)
                args.Add(new Token(funcargs[i].Trim(), TokenType.VARIABLE));
            return args;
        }

        /// <summary>
        /// プログラム関数の引数を関数側にコピー
        /// </summary>
        /// <param name="src">呼出側引数</param>
        /// <param name="dest">関数側引数</param>
        /// <param name="script">関数 Script</param>
        private void setFuncArg(List<Token> src, List<Token> dest, Script script)
        {
            for (int i = 0; i < src.Count; i++) {
                if (0 <= src[i].mValue.IndexOf("[]")) {
                    setFuncArray(src[i], dest[i], script);
                } else {
                    List<Token> variables = mLexer.tokenList(src[i].mValue);
                    string buf = "";
                    for (int j = 0; j < variables.Count; j++) {
                        if (variables[j].mType == TokenType.VARIABLE ||
                            variables[j].mType == TokenType.ARRAY)
                            buf += getValueToken(variables[j].mValue).mValue;
                        else
                            buf += variables[j].mValue;
                    }
                    script.mParse.addVariable(dest[i], express(new Token(buf, TokenType.LITERAL)));
                }
            }
        }

        /// <summary>
        /// 引数の数式を変換した引数にする
        /// </summary>
        /// <param name="arg">引数</param>
        /// <returns>引数文字列</returns>
        public Token cnvArgData(Token arg)
        {
            List<Token> tokens = mLexer.splitArgList(arg.mValue);
            string buf = "";
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].mType == TokenType.EXPRESS) {
                    buf += express(tokens[i]).mValue;
                } else if (tokens[i].mType == TokenType.VARIABLE &&
                    !(i + 1 < tokens.Count && tokens[i + 1].mValue == "[")) {
                    if (mParse.mVariables.ContainsKey(tokens[i].mValue))
                        buf += mParse.mVariables[tokens[i].mValue];
                    else
                        buf += tokens[i].mValue;
                } else {
                    buf += tokens[i].mValue;
                }
            }
            return new Token(buf, TokenType.VARIABLE);
        }

        /// <summary>
        /// 引数をリストに変換( '(a,b,c)' →  'a','b','c' )
        /// </summary>
        /// <param name="arg">引数</param>
        /// <returns>引数リスト</returns>
        private List<Token> cnvArgList(Token arg)
        {
            List<Token> argValue = new List<Token>();
            List<string> args = mLexer.commaSplit(mLexer.stripBracketString(arg.mValue, '('));
            for (int i = 0; i < args.Count; i++) {
                if (0 <= args[i].IndexOf("["))
                    argValue.Add(new Token(args[i].Trim(), TokenType.VARIABLE));
                else
                    argValue.Add(express(new Token(args[i].Trim(), TokenType.EXPRESS)));
            }
            return argValue;
        }

        /// <summary>
        /// 変数名を数値に変換
        /// </summary>
        /// <param name="token">変数名(トークン)</param>
        /// <returns>数値(トークン)</returns>
        private Token getVariable(Token token)
        {
            if (mParse.mVariables.ContainsKey(token.mValue)) {
                return mParse.mVariables[token.mValue];
            } else {
                return token.copy();
            }
        }

        /// <summary>
        /// 変数または配列変数を数値に変換
        /// </summary>
        /// <param name="value">変数または配列変数</param>
        /// <returns>数値</returns>
        public Token getValueToken(string value)
        {
            string buf = "";
            int sp = value.IndexOf("[");
            if (0 <= sp) {
                List<Token> tokens = mLexer.splitArgList(value);
                buf = tokens[0].mValue;
                for (int i = 1; i < tokens.Count; i++) {
                    if (tokens[i].mType == TokenType.VARIABLE ||
                        tokens[i].mType == TokenType.EXPRESS) {
                        buf += express(tokens[i]).mValue;
                    } else {
                        buf += tokens[i].mValue;
                    }
                }
            } else {
                buf = express(new Token(value, TokenType.VARIABLE)).mValue;
            }

            if (mParse.mVariables.ContainsKey(buf))
                return mParse.mVariables[buf];
            else
                return new Token(buf, TokenType.LITERAL);
        }

        /// <summary>
        /// ^配列変数名の変換([m,n] → [2,3])
        /// </summary>
        /// <param name="token">変数名</param>
        /// <returns>変数名</returns>
        private Token getArrayVariable(Token token) 
        {
            char[] sep = new char[] { '[', ']' };
            if (0 > token.mValue.IndexOf("[")) {
                //  配列以外
                return token.copy();
            } else if (0 <= token.mValue.IndexOf("[]")) {
                //  1次元配列宣言
                return null;
            } else if (0 <= token.mValue.IndexOf("[,]")) {
                //  2次元配列宣言
                return null;
            } else {
                //  配列の個別インデックス変換([m,n] → [2,3])
                string[] str = token.mValue.Split(sep);
                if (2 <= str.Length) {
                    List<Token> tokens = mLexer.tokenList(str[1]);
                    List<List<Token>> elements = mLexer.tokensList(tokens, ',');
                    string buf = "";
                    for (int i = 0; i < elements.Count; i++) {
                        buf += express(elements[i]).mValue + ",";
                    }
                    buf = buf.TrimEnd(',');
                    buf = str[0] + '[' + buf + ']';
                    return new Token(buf, TokenType.VARIABLE);
                }
            }
            return token.copy();
        }

        /// <summary>
        /// 配列の一括値設定(a[] = { 1,2,1,3} )
        /// </summary>
        /// <param name="tokens">設定文(トークンリスト)</param>
        /// <returns>可否</returns>
        private bool setArrayData(List<Token> tokens)
        {
            Token name = tokens[0];
            Token data = tokens[2];
            if (0 <= name.mValue.IndexOf("[]"))
                return setArrayData(name, data);
            else if (0 <= name.mValue.IndexOf(",]"))
                return setArrayData2(name, data);
            return false;
        }

        /// <summary>
        /// 1次元配列の一括値設定(a[] = { 1,2,1,3} )
        /// </summary>
        /// <param name="name">配列名</param>
        /// <param name="data">設定値</param>
        /// <returns></returns>
        private bool setArrayData(Token name, Token data)
        {
            if (0 > data.mValue.IndexOf("{"))
                return false;
            string arrayName = name.mValue.Substring(0, name.mValue.IndexOf('['));
            string str = mLexer.stripBracketString(data.mValue, '{');
            List<Token> datas = mLexer.tokenList(str);
            List<List<Token>> dataList = mLexer.tokensList(datas);
            for (int i = 0; i < dataList.Count; i++) {
                    string buf = $"{arrayName}[{i}]";
                    mParse.addVariable(new Token(buf, TokenType.VARIABLE), express(dataList[i]));
            }
            return true;
        }

        /// <summary>
        /// 2次元配列の一括値設定(a[,] = { { 1,2,1,3}, {2,3,4,5} } )
        /// </summary>
        /// <param name="name">配列名</param>
        /// <param name="data">設定値</param>
        /// <returns></returns>
        private bool setArrayData2(Token name, Token data)
        {
            if (0 > data.mValue.IndexOf("{"))
                return false;
            string arrayName = name.mValue.Substring(0, name.mValue.IndexOf('['));
            string str = mLexer.stripBracketString(data.mValue, '{');
            List<string> strings = mLexer.getBracketStringList(str, 0, '{');
            if (0 <= name.mValue.IndexOf("[,]")) {
                for (int i = 0; i < strings.Count; i++) {
                    List<Token> datas = mLexer.tokenList(mLexer.stripBracketString(strings[i], '{'));
                    List<List<Token>> dataList = mLexer.tokensList(datas);
                    for (int j = 0; j < dataList.Count; j++) {
                        string buf = $"{arrayName}[{i},{j}]";
                        mParse.addVariable(new Token(buf, TokenType.VARIABLE), express(dataList[j]));
                    }
                }
            } else {
                int sp = name.mValue.IndexOf("[") + 1;
                int ep = name.mValue.IndexOf(",]");
                string index = name.mValue.Substring(sp, ep - sp);
                List<Token> indexList = mLexer.tokenList(index);
                Token indexToken = express(indexList[0]);
                List<Token> datas = mLexer.tokenList(str);
                List<List<Token>> dataList = mLexer.tokensList(datas);
                for (int i = 0; i < dataList.Count; i++) {
                    string buf = $"{arrayName}[{indexToken.mValue},{i}]";
                    mParse.addVariable(new Token(buf, TokenType.VARIABLE), express(dataList[i]));
                }
            }
            return true;
        }

        /// <summary>
        /// プログラム関数の戻り値受け渡し
        /// </summary>
        /// <param name="src">関数側の戻り値</param>
        /// <param name="dest">呼出し側の変数</param>
        /// <param name="parse">関数 Script</param>
        private void getFuncArray(Token src, Token dest, Script parse)
        {
            if (src == null || dest == null || parse == null) return;
            int sp = src.mValue.IndexOf("[]");
            int dp = dest.mValue.IndexOf("[]");
            if (sp < 0 || dp < 0) return;
            string srcName = src.mValue.Substring(0, sp);
            string destName = dest.mValue.Substring(0, dp);
            foreach (var variable in parse.mParse.mVariables) {
                if (variable.Key.IndexOf(srcName) >= 0) {
                    string key = variable.Key.Replace(srcName, destName);
                    mParse.addVariable(new Token(key, TokenType.VARIABLE), variable.Value);
                }
            }
        }

        /// <summary>
        /// プログラム関数の引数受け渡し
        /// </summary>
        /// <param name="src">呼出し元引数</param>
        /// <param name="dest">関数側引数</param>
        /// <param name="script">関数 Script</param>
        private void setFuncArray(Token src, Token dest, Script script)
        {
            int sp = src.mValue.IndexOf("[]");
            int dp = dest.mValue.IndexOf("[]");
            if (sp < 0 || dp < 0) return;
            string srcName = src.mValue.Substring(0, sp);
            string destName = dest.mValue.Substring(0, dp);
            foreach (var variable in mParse.mVariables) {
                if (variable.Key.IndexOf(srcName) >= 0) {
                    string key = variable.Key.Replace(srcName, destName);
                    script.mParse.addVariable(new Token(key, TokenType.VARIABLE), variable.Value);
                }
            }
        }

        /// <summary>
        /// デバッグ用トークンリスト表示
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="type">VALUEとTYPE両表示</param>
        /// <param name="debug">非表示</param>
        /// <param name="console">コンソール出力</param>
        private void printToken(string title, List<Token> tokens, bool type = false, bool debug = false, bool console = false)
        {
            if (!debug) return;
            if (!console) {
                System.Diagnostics.Debug.Write($"{title} ");
                if (type) 
                    tokens.ForEach(p => System.Diagnostics.Debug.Write($"{p} "));
                else
                    tokens.ForEach(p => System.Diagnostics.Debug.Write($"{p.mValue} "));
                System.Diagnostics.Debug.WriteLine("");
            } else {
                Console.Write($"{title} ");
                tokens.ForEach(p => Console.Write($"{p.mValue} "));
                Console.WriteLine("");
            }
        }
        
        /// <summary>
        /// デバッグ用トークンリストの文字列か
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>文字列</returns>
        private string tokensString(List<Token> tokens)
        {
            string buf = "";
            foreach (var token in tokens)
                buf += token.mValue + " ";
            buf.Trim();
            return buf;
        }
    }
}
