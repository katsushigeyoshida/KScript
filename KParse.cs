using CoreLib;


namespace KScript
{
    /// <summary>
    /// 構文解析()
    /// VARIABLE 変数設定(計算処理) : variable = express
    ///                 array : VARIABLE[VARIABLE] = express
    /// FUNCTION 関数 : function ( arg, .. ) { steatement; steatement; ... } (function変数に結果が入る)
    /// STATEMENT 文  : while ( conditional ) { steatement; steatement; ... }
    ///                 if (conditional ) { steatement; steatement; ... } else { steatement; steatement; ... }
    ///                 for ( initial; conditional; iteration) { statement, statement... }
    ///                 return express ;
    ///                 break ;
    ///                 continue ;
    ///                 print( arg );
    /// EXPRESS文     : 数式
    /// 関数の構成   Parse(script)   :   スクリプトの登録
    ///                 getStatements           トークンを構文単位にまとめる
    ///              execute(function Name, argument)   スクリプトの実行
    ///                 getStatement                指定関数の構文の取得
    ///                 addVariable                 引数の登録
    ///                 exeStatement                構文の実行
    ///                     letStatement            代入文(変数名/配列 = statement ;)
    ///                         getVariableName         変数名/配列名の設定
    ///                         setArrayData            配列データの一括設定( a[] = { 1,2.. }
    ///                         funcStatement           関数処理
    ///                         express                 数式処理
    ///                         addVariable             結果を左辺変数に設定
    ///                     ifStatement             if文(if (condition) steatement/{statement..} [ else steatement/{statement..} ]
    ///                         conditinalStatement     条件判断
    ///                         getStatement            構文の抽出(文/{文..})
    ///                         exeStatements            構文の実行
    ///                     whileStatement          while文(while (condition) steatement/{statement..}
    ///                         conditinalStatement     条件判断
    ///                         exeStatements           構文の実行
    ///                     forStatement            for 文(for(initial; condition; iteration) steatement/{statement..}
    ///                         getStatements           構文に分解
    ///                         letStatement            代入文処理
    ///                         conditinalStatement     条件判断
    ///                         exeStatements           構文実行
    ///                     printStatement          print文 (print(var, var...);
    ///                         express                 数式処理
    ///                     returnStatement         return文 (return var/express;
    ///                         getVariable             返数データの取得
    ///                         addVariable             返数データの登録
    ///                     funcStatement           関数処理
    ///                         innerFunc               内部関数
    ///                         programFunc             プログラム関数
    ///                         expressFunc             数式関数
    ///                         
    ///             programFunc                     コードで書かれた関数の実行
    ///                 Parse
    ///                 getFuncArgs                     関数の引数をList形式に変換
    ///                 setFuncArg                      呼び出し側の引数を関数側の引数に変換
    ///                 getFuncArray                    配列を別の配列に移す(parseからCurrentへ)
    ///             expressFunc                     数式の関数処理
    ///                 getVariable                     引数をList形式に変換
    ///                 express                         数式処理
    ///                 mCalc.expression                数式処理の実行
    ///                 
    ///             getVariableName                 変数名の変換(配列変数名の変換)a[n+1] → a[2]
    ///                 express                         数式処理
    ///             conditinalStatement             条件判断
    ///                 cnvExpress
    ///             getStatements                   構文に分解
    ///                 getStatement                    1ステートメントの構成文を抽出
    ///             getStatement                    構文の抽出(文/{文..})
    ///             exeStatements                   構文の実行(複数)
    ///                 exeStatement                    構文の実行
    ///             setArrayData                    配列データの一括設定( a[] = { 1,2.. }
    ///                 addVariable                     変数データの登録
    ///             getFuncArgs                     関数の引数をList形式に変換
    ///                 setFuncArray                    配列を別の配列に移す(Currentからparseへ)
    ///             setFuncArg                      呼び出し側の引数を関数側の引数に変換
    ///                 getValueToken                   変数を値に変換
    ///             getFuncArray                    配列を別の配列に移す(parseからCurrentへ)
    ///                 addVariable                     変数の登録
    ///             express                          数式処理
    ///                 express
    ///                     isExpress                   数式のみか判断
    ///                     cnvExpress                  数式処理
    ///                     cnvString                   文字列処理
    ///                         cnvArrayName                配列変数を変数に変換(array[a] → array[2])
    ///             cnvExpress                      数式処理
    ///                  funcStatement
    ///                  cnvExpress
    ///                  YCalc.expression
    ///             cnvString                       文字列処理
    ///                  cnvArrayName                   配列変数を変数に変換(array[a] → array[2])
    ///             getVariable                     引数をList形式に変換("(a,b,c..)" → a,b,c... )
    ///             addVariable                     変数の登録(変数名と数値)
    ///             addFunction                     関数の登録
    ///              
    ///             追加関数
    ///             innerFunc                       内部関数
    ///                 getArrayStart                   配列の開始位置(内部関数)
    ///                     getArrayName                    引数の配列名のみを抽出(abc[] → abc)
    ///                 getArraySize                    配列のサイズの取得(内部関数)
    ///                     getArrayName                    引数の配列名のみを抽出(abc[] → abc)
    ///                 arrayClear                      配列をクリア(内部関数)
    ///                     getArrayName                    引数の配列名のみを抽出(abc[] → abc)
    ///                 
    ///             printToken                      デバッグ用 Token List の出力
    ///              
    /// 問題点  引数のアドレス渡し
    ///         グローバル変数の対応
    ///         コールバック関数をサポートできるか?
    ///         
    /// </summary>
    public class KParse
    {
        public Dictionary<string, Token> mVariables = new Dictionary<string, Token>();  //  変数リスト(変数名,値)
        public Dictionary<string, Token> mFunctions = new Dictionary<string, Token>();  //  関数リスト(関数名,(関数式))

        public List<Token> mTokenList = new List<Token>();
        public List<List<Token>> mStatements = new List<List<Token>>();
        public enum RETURNTYPE { NORMAL, CONTINUE, BREAK, RETURN }

        private bool mDebug = false;
        private bool mError = false;
        private string mErrorMessage = "";
        private KLexer mLexer = new KLexer();
        private YCalc mCalc = new YCalc();

        public KParse() { }

        /// <summary>
        /// コンストラクタ
        /// スクリプトを関数ごとに登録
        /// </summary>
        /// <param name="script">スクリプト</param>
        public KParse(string script)
        {
            //  字句解析
            mTokenList.AddRange(mLexer.tokenList(script));
            //  スクリプト登録(mFunctionsに登録)
            mStatements = getStatements(mTokenList);
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
            if (mFunctions.ContainsKey(funcName)) {
                Token func = mFunctions[funcName];
                List<Token> funcList = getStatement(mLexer.tokenList(func.mValue));
                funcStatement = getStatements(mLexer.tokenList(mLexer.stripBracketString(funcList[2].mValue, funcList[2].mValue[0])));
                //  引数の登録
                if (arg != null) {
                    string[] funcargs = mLexer.stripBracketString(funcList[1].mValue, '(').Split(',');
                    for (int i = 0; i < funcargs.Length; i++)
                        addVariable(new Token(funcargs[i].Trim(), TokenType.VARIABLE), arg[i]);
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
        /// 複数ステートメントの実行
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        private RETURNTYPE exeStatements(List<Token> tokens, int sp)
        {
            printToken("exeStatements", tokens);
            RETURNTYPE returnType = RETURNTYPE.NORMAL;
            List<Token> tokenList = new List<Token>();
            if (tokens[sp].mValue[0] == '{')
                tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[sp].mValue, tokens[sp].mValue[0]));
            else
                tokenList.AddRange(tokens.Skip(sp));
            List<List<Token>> statements = getStatements(tokenList);
            foreach (var statement in statements) {
                returnType = exeStatement(statement);
                if (returnType != RETURNTYPE.NORMAL)
                    break;
            }

            return returnType;
        }

        /// <summary>
        /// ステートメントの実行
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public RETURNTYPE exeStatement(List<Token> tokens)
        {
            printToken("exeStatement", tokens);
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
                string buf = funcStatement(tokens[0].mValue, tokens[1], null);
                addVariable(new Token("return", TokenType.VARIABLE), new Token(buf, TokenType.LITERAL));
            } else if (tokens[0].mType == TokenType.COMMENT) {
                System.Diagnostics.Debug.WriteLine($"Comment: {tokens[0].mValue} ");
            } else {
                System.Diagnostics.Debug.WriteLine($"exeStatement: {tokens} ");
            }
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// 計算処理(let ステートメント)
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public void letStatement(List<Token> tokens)
        {
            printToken("letStatement", tokens);
            tokens[0] = getVariableName(tokens[0]);
            if (tokens[1].mValue == "=") {
                if (0 <= tokens[0].mValue.IndexOf("[]") && tokens[2].mType == TokenType.STATEMENT) {
                    setArrayData(tokens[0], tokens[2]);
                } else {
                    Token buf = null;
                    if (tokens[2].mType == TokenType.STRING) {
                        buf = new Token(tokens[2].mValue, TokenType.STRING);
                    } else if (tokens[2].mType == TokenType.FUNCTION) {
                        buf = new Token(funcStatement(tokens[2].mValue, tokens[3], tokens[0]), TokenType.LITERAL);
                    } else {
                        buf = express(tokens, 2);
                    }
                    if (0 > tokens[0].mValue.IndexOf("[]"))
                        addVariable(tokens[0], buf);
                }
            } else if (0 <= Array.IndexOf(Token.multiOperators, tokens[1].mValue)) {
                //  複合演算子(++.--,+=,-=,*=,/=,^=)
                Token token = new Token("1", TokenType.LITERAL);
                if (2 < tokens.Count && tokens[2].mValue != ";")
                    token = express(tokens, 2);
                List<Token> tokenList = new List<Token>(){ 
                    tokens[0], new Token(tokens[1].mValue[0].ToString(), TokenType.OPERATOR), token
                };
                Token buf = express(tokenList, 0);
                addVariable(tokens[0], buf);
            } else {
                addVariable(tokens[0]);
            }
            calcError("letStatement", tokens);
        }

        /// <summary>
        /// ifステートメント if ( conditional ) { statement } else { statement }
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public RETURNTYPE ifStatement(List<Token> tokens)
        {
            printToken("ifStatement", tokens);
            if (conditinalStatement(tokens, 1)) {
                List<Token> tokenList = getStatement(tokens, 2);
                return exeStatements(tokenList, 0);
            } else {
                int n = tokens.FindIndex(p => p.mValue == "else");
                if (0 < n) {
                    List<Token> tokenList = getStatement(tokens, n + 1);
                    return exeStatements(tokenList, 0);
                }
            }
            calcError("ifStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// whileステートメント while (conditional) { statement }
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public RETURNTYPE whileStatement(List<Token> tokens)
        {
            printToken("whileStatement", tokens);
            while (conditinalStatement(tokens, 1)) {
                RETURNTYPE returnType = exeStatements(tokens, 2);
                if (returnType == RETURNTYPE.BREAK) break;
                else if (returnType == RETURNTYPE.RETURN) return RETURNTYPE.RETURN;
            }
            calcError("whileStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// for ステートメント for ( variable; conditional; express) { statement }
        /// </summary>
        /// <param name="tokens"></param>
        public RETURNTYPE forStatement(List<Token> tokens)
        {
            printToken("forStatement", tokens);
            List<Token> conditionList = mLexer.tokenList(mLexer.stripBracketString(tokens[1].mValue));
            List<List<Token>> funcStatement = getStatements(conditionList);
            if (funcStatement.Count == 3) {
                letStatement(funcStatement[0]);                 //  初期値
                while (conditinalStatement(funcStatement[1])) { //  条件
                    RETURNTYPE returnType = exeStatements(tokens, 2);
                    if (returnType == RETURNTYPE.BREAK) break;
                    else if (returnType == RETURNTYPE.RETURN) return RETURNTYPE.RETURN;
                    letStatement(funcStatement[2]);             //  更新処理
                }
            }
            calcError("forStatement", tokens);
            return RETURNTYPE.NORMAL;
        }

        /// <summary>
        /// print 文 ( print(a, b, ...); )
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public void printStatement(List<Token> tokens)
        {
            printToken("printStatement", tokens);
            if (tokens.Count <= 1) {
                Console.WriteLine();
            } else if (tokens[1].mType == TokenType.EXPRESS) {
                List<Token> tokenList = mLexer.tokenList(mLexer.stripBracketString(tokens[1].mValue));
                if (tokenList == null || tokenList.Count == 0)
                    Console.WriteLine();
                else
                    for (int i = 0; i < tokenList.Count; i++)
                        if (tokenList[i].mType != TokenType.DELIMITER)
                            Console.Write(express(tokenList[i]).mValue);
            } else if (tokens[1].mType == TokenType.STRING) {
                Console.Write(tokens[1].mValue);
            }
            calcError("printStatement", tokens);
        }

        /// <summary>
        /// return ステートメント
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        public void returnStatement(List<Token> tokens)
        {
            printToken("returnStatement", tokens);
            List<Token> buf = getVariable(tokens[1]);
            addVariable(new Token("return", TokenType.VARIABLE), buf[0]);
        }

        /// <summary>
        /// 関数の実行
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数(a,b,.. )</param>
        /// <returns></returns>
        public string funcStatement(string funcName, Token arg, Token ret)
        {
            System.Diagnostics.Debug.WriteLine($"funcStatement {funcName} {arg} {ret}");
            string result = innerFunc(funcName, arg);   //  内部関数処理
            if (result != "") {
                return result;
            } else if (mFunctions.ContainsKey(funcName)) {
                //  プログラム関数処理
                return programFunc(funcName, arg, ret);
            } else {
                //  数式の関数処理
                return expressFunc(funcName, arg);
            }

            ////  関数 conditionList[0]:関数名 conditionList[1]:引数 conditionList[2]:処理部
            //List<Token> conditionList = mLexer.tokenList(mFunctions[funcName].mValue);
            ////  引数の登録
            //string[] funcargs = mLexer.stripBracketString(conditionList[1].mValue, '(').Split(',');
            //for (int i = 0; i < funcargs.Length; i++)
            //    addVariable(new Token(funcargs[i].Trim(), TokenType.VARIABLE), arg[i]);
            ////  関数の実行
            //List<Token> func = mLexer.tokenList(mLexer.stripBracketString(conditionList[2].mValue, conditionList[2].mValue[0]));
            //List<List<Token>> funcStatement = getStatements(func);
            //foreach (var tokenList in funcStatement)
            //    exeStatement(tokenList);
            //return mVariables["return"].mValue;
        }

        /// <summary>
        /// 追加内部関数(内部関数)
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>処理結果</returns>
        private string innerFunc(string funcName, Token arg)
        {
            List<Token> args = getVariable(arg);
            switch (funcName) {
                case "input": return Console.ReadLine();
                case "arrayStart": return getArrayStart(args).ToString();
                case "arraySize": return getArraySize(args).ToString();
                case "arrayClear": arrayClear(args); return " ";
                default: break;
            }
            return "";
        }

        /// <summary>
        /// 配列の開始位置(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>開始位置</returns>
        private int getArrayStart(List<Token> args)
        {
            string arrayName = getArrayName(args[0]);
            int start = int.MaxValue;
            foreach (var variable in mVariables) {
                int sp = variable.Key.IndexOf("[");
                if (0 <= sp) {
                    if (getArrayName(new Token(variable.Key, TokenType.VARIABLE)) == arrayName) {
                        int ep = variable.Key.IndexOf("]");
                        if (sp + 1 < ep) {
                            string infix = variable.Key.Substring(sp + 1, ep - sp - 1);
                            int arrayNo = int.Parse(infix);
                            if (arrayNo < start)
                                start = arrayNo;
                        }
                    }
                }
            }
            if (start == int.MaxValue)
                start = 0;
            return start;
        }

        /// <summary>
        /// 配列のサイズの取得(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        /// <returns>サイズ</returns>
        private int getArraySize(List<Token> args)
        {
            string arrayName = getArrayName(args[0]);
            int count = 0;
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf("[")) {
                    if (getArrayName(new Token(variable.Key, TokenType.VARIABLE)) == arrayName)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 配列をクリア(内部関数)
        /// </summary>
        /// <param name="args">配列名</param>
        private void arrayClear(List<Token> args)
        {
            string arrayName = getArrayName(args[0]);
            int count = 0;
            foreach (var variable in mVariables) {
                if (0 <= variable.Key.IndexOf("[")) {
                    if (getArrayName(new Token(variable.Key, TokenType.VARIABLE)) == arrayName)
                        mVariables.Remove(variable.Key);
                }
            }
        }

        /// <summary>
        /// コードで書かれた関数の実行
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <param name="ret">return変数</param>
        /// <returns>処理結果</returns>
        private string programFunc(string funcName, Token arg, Token ret)
        {
            KParse parse = new KParse(mFunctions[funcName].mValue);
            parse.mFunctions = mFunctions;
            List<Token> srcArgs = getFuncArgs(arg.mValue);
            List<Token> outArgs = getFuncArgs(mFunctions[funcName].mValue, 1);
            setFuncArg(srcArgs, outArgs, parse);
            parse.execute(funcName, null);
            if (parse.mVariables.ContainsKey("return")) {
                getFuncArray(parse.mVariables["return"], ret, parse);
                return parse.mVariables["return"].mValue;
            } else
                return "";
        }

        /// <summary>
        /// 数式の関数処理
        /// </summary>
        /// <param name="funcName">関数名</param>
        /// <param name="arg">引数</param>
        /// <returns>計算結果</returns>
        private string expressFunc(string funcName, Token arg)
        {
            List<Token> argValue = getVariable(arg);
            string buf = "";
            foreach (Token token in argValue)
                buf += express(token).mValue + ",";
            buf = buf.TrimEnd(',');
            string result = mCalc.expression($"{funcName}({buf})").ToString();
            calcError(funcName, argValue);
            return result;
        }

        /// <summary>
        /// 制御文
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">条件文の位置</param>
        /// <returns>判定(true/false)</returns>
        public bool conditinalStatement(List<Token> tokens, int sp = 0)
        {
            printToken("conditinalStatement", tokens);
            if (2 < tokens.Count && tokens[1].mType != TokenType.CONDITINAL)
                tokens = mLexer.tokenList(mLexer.stripBracketString(tokens[sp].mValue));
            else if (tokens.Count == 1)
                tokens = mLexer.tokenList(mLexer.stripBracketString(tokens[0].mValue));
            List<Token> a = new List<Token>();
            List<Token> b = new List<Token>();
            int n = 0;
            while (n < tokens.Count && tokens[n].mType != TokenType.CONDITINAL)
                a.Add(tokens[n++]);
            Token cond = tokens[n++];
            while (n < tokens.Count)
                b.Add(tokens[n++]);
            switch (cond.mValue) {
                case "||": return conditinalStatement(a) || conditinalStatement(b);
                case "&&": return conditinalStatement(a) && conditinalStatement(b);
                case "!" : return !conditinalStatement(b);
            }

            double avalue = double.Parse(cnvExpress(a, 0));
            double bvalue = double.Parse(cnvExpress(b, 0));
            switch (cond.mValue) {
                case "==": return avalue == bvalue;
                case "!=": return avalue != bvalue;
                case "<": return avalue < bvalue;
                case ">": return avalue > bvalue;
                case "<=": return avalue <= bvalue;
                case ">=": return avalue >= bvalue;
            }
            return true;
        }

        /// <summary>
        /// ステートメントごとにトークンリストを抽出
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <returns>ステートメントリスト</returns>
        public List<List<Token>> getStatements(List<Token> tokens)
        {
            List<List<Token>> statemets = new List<List<Token>>();
            for (int i = 0; i < tokens.Count; i++) {
                List<Token> statement = new List<Token>();
                int sp = i, ep = i;
                switch (tokens[i].mType) {
                    case TokenType.VARIABLE:
                    case TokenType.ARRAY:
                        //  代入文 (変数 = 数式 ;)
                        while (i < tokens.Count && tokens[i].mValue != ";" && 0 > tokens[i].mValue.IndexOf("}")) i++;
                        if (i < tokens.Count)
                            statement.AddRange(tokens.GetRange(sp, i - sp + 1));
                        else
                            statement.AddRange(tokens.GetRange(sp, i - sp));
                        break;
                    case TokenType.FUNCTION:
                        //  関数 (関数構文を登録)
                        Token functionName = tokens[i];
                        statement.Add(tokens[i++]);             //  関数名
                        statement.Add(tokens[i++]);             //  (...) 引数
                        if (tokens[i].mValue == ";") {
                            statement.Add(tokens[i]);
                        } else {
                            List<Token> statements = getStatement(tokens, i);   //  {...} 処理文
                            statement.AddRange(statements);
                            addFunction(functionName, statement);
                            statement.Clear();
                        }
                        break;
                    case TokenType.STATEMENT:
                        //  制御文
                        statement.Add(tokens[i]);
                        if (tokens[i].mValue == "while") {
                            //  while文 (while (conditional) { 文... }
                            statement.Add(tokens[++i]);         //  (...) 制御文
                            List<Token> stateList = getStatement(tokens, ++i);
                            statement.AddRange(stateList);      //  {文...}/文 処理文
                            i += stateList.Count - 1;
                        } else if (tokens[i].mValue == "for") {
                            //  for 文
                            statement.Add(tokens[++i]);         //  (...) 制御文
                            List<Token> stateList = getStatement(tokens, ++i);
                            statement.AddRange(stateList);      //  {文...}/文 処理文
                            i += stateList.Count - 1;
                        } else if (tokens[i].mValue == "if") {
                            //  if文 (if (conditional) 文 / { 文 } else 文 / { 文 }
                            statement.Add(tokens[++i]);         //  (...) 制御文
                            List<Token> stateList = getStatement(tokens, ++i);
                            statement.AddRange(stateList);      //  {文...}/文 処理文
                            i += stateList.Count - 1;
                            if (i < tokens.Count && tokens[i].mValue == "else") {
                                statement.Add(tokens[i]);       //  else
                                stateList = getStatement(tokens, ++i);
                                statement.AddRange(stateList);  //  {文...}/文 処理分
                                i += stateList.Count - 1;
                            }
                        } else if (tokens[i].mValue == "return" ||
                            tokens[i].mValue == "break" ||
                            tokens[i].mValue == "continue") {
                            //  return 文,break文,continue文
                            List<Token> stateList = getStatement(tokens, ++i);
                            statement.AddRange(stateList);      //  {文...}/文 処理分
                            i += stateList.Count - 1;
                        } else {
                            //  単なる文
                            i++;
                            if (i < tokens.Count && (tokens[i].mValue[0] == '(' || tokens[i].mValue[0] == '{'))
                                statement.Add(tokens[i]);
                            else
                                i--;
                        }
                        break;
                    case TokenType.COMMENT:
                        //System.Diagnostics.Debug.WriteLine($"Comment: {tokens[i]} ");
                        break;
                    default:
                        //System.Diagnostics.Debug.WriteLine($"default: {tokens[i]} ");
                        break;
                }
                if (0 < statement.Count)
                    statemets.Add(statement);
            }
            return statemets;
        }

        /// <summary>
        /// 1ステートメントの構成文を抽出
        /// ({ }で囲まれた部分または ; までのステートメント)
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>ステートメント構成リスト</returns>
        public List<Token> getStatement(List<Token> tokens, int sp = 0)
        {
            List<Token> statement = new List<Token>();
            int ep = sp;
            while (ep < tokens.Count) {
                if (tokens[ep].mValue[0] == '{') {
                    statement.Add(tokens[ep]);
                    break;
                } else if (tokens[ep].mValue == ";") {
                    statement.Add(tokens[ep]);
                    break;
                } else {
                    statement.Add(tokens[ep]);
                }
                ep++;
            }
            return statement;
        }

        /// <summary>
        /// 計算処理の実行
        /// </summary>
        /// <param name="token">トークン</param>
        /// <returns>計算結果</returns>
        public Token express(Token token)
        {
            return express(new List<Token>() { token }, 0);
        }

        /// <summary>
        /// 計算処理の実行
        /// </summary>
        /// <param name="tokens">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>計算結果</returns>
        public Token express(List<Token> tokens, int sp)
        {
            if (isExpress(tokens, sp)) {
                return new Token(cnvExpress(tokens, sp), TokenType.LITERAL);
            } else {
                return new Token(cnvString(tokens, sp), TokenType.STRING);
            }
        }

        /// <summary>
        /// 数式のみか判断
        /// </summary>
        /// <param name="tokenList">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>判定</returns>
        private bool isExpress(List<Token> tokenList, int sp = 0)
        {
            for (int i = sp; i < tokenList.Count; i++) {
                if (tokenList[i].mType == TokenType.STRING) {
                    return false;
                } else if (tokenList[i].mType == TokenType.VARIABLE ||
                    tokenList[i].mType == TokenType.ARRAY) {
                    Token variable = getVariableName(tokenList[i]);
                    if (mVariables.ContainsKey(variable.mValue) &&
                        mVariables[variable.mValue].mType == TokenType.STRING)
                        return false;
                } else if (tokenList[i].mType == TokenType.FUNCTION) {

                } else if (tokenList[i].mType == TokenType.EXPRESS) {
                    List<Token> tokens = mLexer.tokenList(mLexer.stripBracketString(tokenList[i].mValue));
                    if (!isExpress(tokens))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 数式を計算する(変数の置換えもする)
        /// </summary>
        /// <param name="tokenList">数式のトークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>計算結果</returns>
        private string cnvExpress(List<Token> tokenList, int sp)
        {
            string buf = "";
            for (int i = sp; i < tokenList.Count; i++) {
                if (tokenList[i].mValue == ";" || tokenList[i].mValue == ",") {
                    break;
                } else if (tokenList[i].mType == TokenType.VARIABLE ||
                    tokenList[i].mType == TokenType.ARRAY) {
                    Token variable = getVariableName(tokenList[i]);
                    if (mVariables.ContainsKey(variable.mValue))
                        buf += mVariables[variable.mValue].mValue;
                } else if (tokenList[i].mType == TokenType.FUNCTION) {
                    string funcName = tokenList[i].mValue;
                    buf += funcStatement(funcName, tokenList[i + 1], null);
                    i++;
                } else if (tokenList[i].mType == TokenType.EXPRESS) {
                    //List<Token> tokens = mLexer.tokenList(mLexer.stripBracketString(tokenList[i].mValue));
                    List<Token> tokens = mLexer.tokenList(tokenList[i].mValue);
                    buf += cnvExpress(tokens, 0);
                } else if (tokenList[i].mType == TokenType.STATEMENT) {
                    if (tokenList[0].mValue == "return") {
                        buf += cnvExpress(tokenList, 1);
                        break;
                    }
                } else {
                    buf += tokenList[i].mValue;
                }
            }
            return mCalc.expression(buf).ToString();
        }

        /// <summary>
        /// 文字処理
        /// </summary>
        /// <param name="tokenList">トークンリスト</param>
        /// <param name="sp">開始位置</param>
        /// <returns>文字列</returns>
        private string cnvString(List<Token> tokenList, int sp)
        {
            string buf = "";
            for (int i = sp; i < tokenList.Count; i++) {
                if (tokenList[i].mValue == ";" || tokenList[i].mValue == ",") {
                    break;
                } else if (tokenList[i].mType == TokenType.VARIABLE) {
                    Token variable = cnvArrayName(tokenList[i]);
                    if (mVariables.ContainsKey(variable.mValue))
                        buf += mVariables[variable.mValue].mValue;
                } else if (tokenList[i].mType == TokenType.FUNCTION) {
                    if (!mFunctions.ContainsKey(tokenList[i].mValue)) {
                        buf += tokenList[i].mValue;
                        buf += tokenList[++i].mValue;
                    }
                } else if (tokenList[i].mType == TokenType.EXPRESS) {
                    List<Token> tokens = mLexer.tokenList(mLexer.stripBracketString(tokenList[i].mValue));
                    buf += cnvString(tokens, 0);
                } else if (tokenList[i].mType == TokenType.STRING) {
                    buf += tokenList[i].mValue.Replace("\\n", "\n");
                } else {
                    //buf += tokenList[i].mValue;
                }
            }
            return buf;
        }

        /// <summary>
        /// 変数名の変換(配列変数名の変換)
        /// a[n+1] → a[2]
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Token getVariableName(Token token)
        {
            char[] sep = new char[] { '[', ']' };
            if (0 > token.mValue.IndexOf("[")) {
                return token;
            } else if (0 <= token.mValue.IndexOf("[]")) {
                return token;
            } else {
                string[] str = token.mValue.Split(sep);
                if (2 <= str.Length) {
                    string buf = str[0] + '[' + express(new Token(str[1], TokenType.EXPRESS)).mValue + ']';
                    return new Token(buf, TokenType.VARIABLE);
                }
            }
            return token;
        }

        /// <summary>
        /// 配列の一括設定 ( abc[] = { 1, 2, 3 } )
        /// </summary>
        /// <param name="name">配列名</param>
        /// <param name="data">配列データ</param>
        private void setArrayData(Token name, Token data)
        {
            string str = mLexer.stripBracketString(data.mValue, '{');
            string[] datas = str.Split(',');
            string arrayName = name.mValue.Substring(0, name.mValue.IndexOf('['));
            for (int i = 0; i < datas.Length; i++) {
                string buf = $"{arrayName}[{i}]";
                addVariable(new Token(buf, TokenType.VARIABLE), new Token(datas[i].Trim(), TokenType.LITERAL));
            }
        }

        /// <summary>
        /// 配列変数を変数に変換(array[a] → array[2])
        /// </summary>
        /// <param name="token">トークン</param>
        /// <returns>トークン</returns>
        private Token cnvArrayName(Token token)
        {
            char[] sep = new char[] { '[', ']' };
            int sp = token.mValue.IndexOf('[');
            if (0 <= sp) {
                string[] str = token.mValue.Split(sep);
                if (2 <= str.Length) {
                    if (mVariables.ContainsKey(str[1])) {
                        string buf = str[0] + '[' + mVariables[str[1]].mValue + ']';
                        return new Token(buf, TokenType.VARIABLE);
                    }
                }
            }
            return token;
        }

        /// <summary>
        /// 引数の配列名のみを抽出
        /// </summary>
        /// <param name="arg">引数</param>
        /// <returns>配列名</returns>
        private string getArrayName(Token arg)
        {
            return arg.mValue.Substring(0, arg.mValue.IndexOf('['));
        }

        /// <summary>
        /// 関数の引数をList形式に変換
        /// (a,b,c...) → List<Token> { a,b,c,... } に変換
        /// </summary>
        /// <param name="func">関数引数</param>
        /// <param name="sp">開始位置</param>
        /// <returns>引数リスト</returns>
        private List<Token> getFuncArgs(string func, int sp = 0)
        {
            List<Token> args = new List<Token>();
            List<Token> funcList = getStatement(mLexer.tokenList(func));
            string[] funcargs = mLexer.stripBracketString(funcList[sp].mValue, '(').Split(',');
            for (int i = 0; i < funcargs.Length; i++)
                args.Add(new Token(funcargs[i].Trim(), TokenType.VARIABLE));
            return args;
        }

        /// <summary>
        /// 呼び出し側の引数を関数側の引数に変換
        /// (配列形式 は分解して変換 src[] → dest[1],dest[2]...)
        /// </summary>
        /// <param name="src">呼出し側引数</param>
        /// <param name="dest">関数側引数</param>
        /// <param name="parse">パース</param>
        private void setFuncArg(List<Token> src, List<Token> dest, KParse parse)
        {
            for (int i = 0; i < src.Count; i++) {
                if (0 <= src[i].mValue.IndexOf("[]")) {
                    setFuncArray(src[i], dest[i], parse);
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
                    parse.addVariable(dest[i], express(new Token(buf, TokenType.LITERAL)));
                }
            }
        }

        /// <summary>
        /// 変数を値に変換
        /// </summary>
        /// <param name="value">変数/値</param>
        /// <returns>値</returns>
        private Token getValueToken(string value)
        {
            int sp = value.IndexOf("[");
            if (0 <= sp) {
                int ep = value.IndexOf("]");
                if (sp < ep) {
                    string prefix = value.Substring(0, sp + 1);
                    string infix = value.Substring(sp + 1, ep - sp - 1);
                    string postfix = value.Substring(ep);
                    value = prefix + express(new Token(infix, TokenType.EXPRESS)).mValue + postfix;
                }
            } else {
                value = express(new Token(value, TokenType.VARIABLE)).mValue;
            }

            if (mVariables.ContainsKey(value))
                return mVariables[value];
            else
                return new Token(value, TokenType.LITERAL);
        }

        /// <summary>
        /// 配列を別の配列に移す(Currentからparseへ)
        /// src[] → src[1],src[2]... → dest[1],dest[2]...
        /// </summary>
        /// <param name="src">元の配列</param>
        /// <param name="dest">移動先の配列</param>
        /// <param name="parse">パース</param>
        private void setFuncArray(Token src, Token dest, KParse parse)
        {
            int sp = src.mValue.IndexOf("[]");
            int dp = dest.mValue.IndexOf("[]");
            if (sp < 0 || dp < 0) return;
            string srcName = src.mValue.Substring(0, sp);
            string destName = dest.mValue.Substring(0, dp);
            foreach (var variable in mVariables) {
                if (variable.Key.IndexOf(srcName) >= 0) {
                    string key = variable.Key.Replace(srcName, destName);
                    parse.addVariable(new Token(key, TokenType.VARIABLE), variable.Value);
                }
            }
        }

        /// <summary>
        /// 配列を別の配列に移す(parseからCurrentへ)
        /// src[] → src[1],src[2]... → dest[1],dest[2]...
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="parse"></param>
        private void getFuncArray(Token src, Token dest, KParse parse)
        {
            if (src == null || dest == null || parse == null) return;
            int sp = src.mValue.IndexOf("[]");
            int dp = dest.mValue.IndexOf("[]");
            if (sp < 0 || dp < 0) return;
            string srcName = src.mValue.Substring(0, sp);
            string destName = dest.mValue.Substring(0, dp);
            foreach (var variable in parse.mVariables) {
                if (variable.Key.IndexOf(srcName) >= 0) {
                    string key = variable.Key.Replace(srcName, destName);
                    addVariable(new Token(key, TokenType.VARIABLE), variable.Value);
                }
            }
        }

        /// <summary>
        /// 引数をList形式に変換
        /// ( (a, b,..) →  List<string> { a, b, ... } )
        /// </summary>
        /// <param name="arg">引数トークン</param>
        /// <returns>引数リスト</returns>
        private List<Token> getVariable(Token arg)
        {
            List<Token> argValue = new List<Token>();
            string[] args = mLexer.stripBracketString(arg.mValue, '(').Split(',');
            for (int i = 0; i < args.Length; i++) {
                if (0 <= args[i].IndexOf("[]"))
                    argValue.Add(new Token(args[i].Trim(), TokenType.VARIABLE));
                else
                    argValue.Add(express(new Token(args[i].Trim(), TokenType.EXPRESS)));
            }
            return argValue;
        }

        /// <summary>
        /// 変数の登録(変数名と数値)
        /// </summary>
        /// <param name="key">変数名(トークン)</param>
        /// <param name="value">数値/数式(トークン)</param>
        public void addVariable(Token key, Token value = null)
        {
            if (!mVariables.ContainsKey(key.mValue)) {
                mVariables.Add(key.mValue, value);
            } else {
                mVariables[key.mValue] = value;
            }
        }

        /// <summary>
        /// 関数の登録
        /// </summary>
        /// <param name="key">関数名(トークン)</param>
        /// <param name="value">関数(トークン)</param>
        public void addFunction(Token key, List<Token> func = null)
        {
            if (!mFunctions.ContainsKey(key.mValue)) {
                string tokens = "";
                foreach (Token token in func)
                    tokens += token.mValue + " ";
                mFunctions.Add(key.mValue, new Token(tokens, TokenType.FUNCTION));
            } else {
                mFunctions[key.mValue] = null;
            }
        }

        /// <summary>
        /// Calcエラー
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="tokens">トークン</param>
        private void calcError(string title, List<Token> tokens)
        {
            if (mCalc.mError) {
                Console.WriteLine($"{title}:{mCalc.mErrorMsg}");
                printToken(title, tokens, true);
                mCalc.mError = false;
            }
        }

        /// <summary>
        /// デバッグ用 Token List の出力
        /// </summary>
        /// <param name="title"></param>
        /// <param name="tokens"></param>
        private void printToken(string title, List<Token> tokens, bool debug = false)
        {
            if (mDebug || debug) {
                System.Diagnostics.Debug.Write($"{title}: ");
                tokens.ForEach(p => System.Diagnostics.Debug.Write($"{p} "));
                System.Diagnostics.Debug.WriteLine("");
            } else if (!mDebug && debug) {
                Console.Write($"{title}: ");
                tokens.ForEach(p => Console.Write($"{p.mValue} "));
                Console.WriteLine("");
            }
        }
    }
}
