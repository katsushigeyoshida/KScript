namespace KScript
{
    //  字句の種類
    public enum TokenType
    {
        VARIABLE,               //  変数名
        ARRAY,                  //  配列変数名
        OPERATOR,               //  演算子
        CONDITINAL,             //  条件演算子
        DELIMITER,              //  区切り文字
        LITERAL,                //  固定値(数値)
        STRING,                 //  固定値(文字列)
        STATEMENT,              //  文
        EXPRESS,                //  数式
        FUNCTION,               //  関数
        COMMENT,                //  コメント
    }

    //  字句データ
    public class Token
    {
        public static string[] comment = { "//", "/*", "*/" };
        public static string[] conditionOperator = { 
            "==", "!=", "<", ">", "<=", ">=", "&&", "||", "!" 
        };
        public static string[] multiOperators = { "++", "--", "+=", "-=", "*=", "/=", "^=" };
        public static char[] operators = { 
            '=', '+', '-', '*', '/', '%', '^', '!', '<', '>', '&', '|'
        };
        public static char[] delimiter = { '(', ')', '{', '}', '[', ']', ' ', ',', ';' };
        public static char[] skipChar = { ' ', '\t', '\r', '\n' };
        public static string[] statement = {
            "let", "while", "if", "else", "for", "return", "break", "continue", "print" 
        };

        public string mValue;
        public TokenType mType;

        public Token(string value, TokenType type)
        {
            mValue = value;     //  データの値
            mType = type;       //  データの種類
        }

        public override string ToString()
        {
            return $"{mValue}[{mType.ToString()}]";
        }
    }

    /// <summary>
    /// 字句解析
    /// SKIP CHAR               :  ' ', '\t', '\r', '\n'
    /// コメント  (COMMENT)     : // ... \n, /* ...*/
    /// 計算式    (EXPRESS)     : ( 計算式 )
    /// 文リスト  (STAEMENT)    : { 文; 文; ... }
    /// 文        (STATEMENT)   : let|while|if|else|return|print
    /// 変数名    (VARIABLE)    : [a..z|A..Z][a..z|A..Z|0..9]*
    /// 関数名    (FUNCTION)    : [a..z|A..Z][a..z|A..Z|0..9]* (...)
    /// 配列      (ARRAY)       : (VARIABLE)'['*']'
    /// 数値      (LITERAL)     : [0..9|.|+|-][0..9|.]
    /// 文字列    (STRING)      : "..."
    /// 演算子    (OPERATOR)    : =|+|-|*|/|%|^|!
    /// 条件演算子(CONDITINAL)  : ==|!=|<|>|<=|>=
    /// 区切り文字(DELIMITER)   : (|)|{|}| |,|;
    /// </summary>
    public class KLexer
    {
        //  括弧リスト
        private char[] mBrackets = { '(', ')', '{', '}', '[', ']', '\"', '\"' };

        public KLexer() { }

        /// <summary>
        /// 字句解析 文字列をトークンリストに変換
        /// コメント(COMMENT),括弧(){}(EXPRESS/STATEMENT),変数名(STATEMENT/FUNCTION/VAROABLE)
        /// 数値(LITERAL),文字列(STRING),識別子(演算子(OPERATOR),条件演算子(CONDITINAL))
        /// 区切文字(DELIMITER)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>トークンリスト</returns>
        public List<Token> tokenList(string str)
        {
            List<Token> tokens = new List<Token>();
            for (int i = 0; i < str.Length; i++) {
                string buf = "";
                string twoChar = "";
                if (i + 1 < str.Length)
                    twoChar = str.Substring(i, 2);
                if (0 <= Array.IndexOf(Token.skipChar, str[i])) {
                    //  読み飛ばし
                } else if (twoChar != "" && 0 <= Array.IndexOf(Token.comment, twoChar)) {
                    //  コメント
                    if (twoChar == "//") {
                        while (i < str.Length && (str[i] != '\n' && str[i] != '\r')) buf += str[i++];
                    } else if (twoChar == "/*") {
                        i += 2;
                        while (i < str.Length - 1 && !(str[i] == '*' && str[i + 1] == '/')) buf += str[i++];
                    }
                    if (0 < buf.Length) {
                        tokens.Add(new Token(buf, TokenType.COMMENT));
                        i--;
                    }
                } else if (str[i] == '(' || str[i] == '{') {
                    //  括弧で囲まれた計算式
                    buf = getBracketString(str, i, str[i]);
                    tokens.Add(new Token(buf, str[i] == '{' ? TokenType.STATEMENT : TokenType.EXPRESS));
                    i += buf.Length - 1;
                } else if (Char.IsLetter(str[i])) { //  アルファベットの確認(a-z,A-Z,全角も)
                    //  変数名、予約語
                    while (i < str.Length && Array.IndexOf(Token.operators, str[i]) < 0
                             && Array.IndexOf(Token.delimiter, str[i]) < 0) {
                        if (Array.IndexOf(Token.skipChar, str[i]) < 0)
                            buf += str[i];
                        i++;
                    }
                    while (i < str.Length && str[i] == ' ') i++;    //  空白削除
                    if (0 <= Array.IndexOf(Token.statement, buf)) {
                        //  STATEMENT
                        tokens.Add(new Token(buf, TokenType.STATEMENT));
                    } else if (i < str.Length && str[i] == '[') {
                        //  ARRAY
                        string backet = getBracketString(str, i, str[i]);
                        buf += backet;
                        tokens.Add(new Token(buf, TokenType.ARRAY));
                        i += backet.Length;
                    } else if (i < str.Length && str[i] == '(') {
                        //  FUNCTION
                        tokens.Add(new Token(buf, TokenType.FUNCTION));
                    } else {
                        //  VARIABLE
                        tokens.Add(new Token(buf, TokenType.VARIABLE));
                    }
                    i--;
                } else if (Char.IsNumber(str[i]) || str[i] == '.' ||
                    (i == 0 && (str[i] == '-' || str[i] == '+'))) {
                    //  数値
                    while (i < str.Length && (Char.IsNumber(str[i]) || str[i] == '.' || str[i] == '-')) {
                        if (Array.IndexOf(Token.skipChar, str[i]) < 0)
                            buf += str[i];
                        i++;
                    }
                    if (0 < buf.Length) {
                        tokens.Add(new Token(buf, TokenType.LITERAL));
                        i--;
                    }
                } else if (str[i] == '\"') {
                    //  文字列
                    buf = getBracketString(str, i, str[i]);
                    tokens.Add(new Token(stripBracketString(buf, str[i]), TokenType.STRING));
                    i += buf.Length - 1;
                } else if (twoChar != "" && 0 <= Array.IndexOf(Token.multiOperators, twoChar)) {
                    //  複合演算子
                    tokens.Add(new Token(twoChar, TokenType.OPERATOR));
                    i++;
                } else if (0 <= Array.IndexOf(Token.operators, str[i])) {
                    //  識別子(演算子/条件演算子)
                    if (twoChar != "" && 0 <= Array.IndexOf(Token.conditionOperator, twoChar)) {
                        tokens.Add(new Token(twoChar, TokenType.CONDITINAL));
                        i++;
                    } else if (0 <= Array.IndexOf(Token.conditionOperator, str[i].ToString())) {
                        tokens.Add(new Token(str[i].ToString(), TokenType.CONDITINAL));
                    } else {
                        tokens.Add(new Token(str[i].ToString(), TokenType.OPERATOR));
                    }
                } else if (0 <= Array.IndexOf(Token.delimiter, str[i])) {
                    //  区切文字
                    tokens.Add(new Token(str[i].ToString(), TokenType.DELIMITER));
                } else {
                    System.Diagnostics.Debug.WriteLine($"tokenList: {i} {str.Substring(i)}");
                }
            }
            return tokens;
        }

        /// <summary>
        /// 括弧付き文字列で前後の括弧を除いた文字列
        /// </summary>
        /// <param name="str">括弧付き文字列</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>文字列</returns>
        public string stripBracketString(string str, char bracket = '(')
        {
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return str;
            int sp = str.IndexOf(mBrackets[offset]);
            int ep = str.LastIndexOf(mBrackets[offset + 1]);
            if (sp < 0 || ep < 0)
                return str;
            return str.Substring(sp + 1, ep - sp - 1);
        }

        /// <summary>
        /// 括弧で囲まれた文字列の抽出(括弧含む)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="n">開始位置</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>抽出文字列</returns>
        public string getBracketString(string str, int n, char bracket = '(')
        {
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return str;
            int sp = str.IndexOf(mBrackets[offset], n);
            int pos = sp;
            if (pos < 0) return str;
            int count = 1;
            pos++;
            while (pos < str.Length && 0 < count) {
                if (str[pos] == '\\') pos++;
                else if (str[pos] == mBrackets[offset + 1]) count--;
                else if (str[pos] == mBrackets[offset]) count++;
                pos++;
            }
            return str.Substring(sp, pos - sp);
        }
    }
}
