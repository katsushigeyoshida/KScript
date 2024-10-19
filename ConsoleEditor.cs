using CoreLib;

namespace KScript
{
    /// <summary>
    /// コンソールで使用する行エディタ
    /// </summary>
    public class ConsoleEditor
    {
        public List<string> mScriptData;                    //  編集テキストデータ
        public string mScriptPath = "";                     //  ファイルパス
        public string mDataFolder = "";                     //  ファイル保存フォルダ
        public string mFileExt = "";                        //  ファイルの拡張子
        const string PROMPT = ">> ";                        //  プロンプト
        public Action callback;                             //  コールバック関数

        private YCalc calc = new YCalc();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="folder">ファイルフォルダ</param>
        /// <param name="ext">ファイルの拡張子</param>
        public ConsoleEditor(string folder, string ext)
        {
            mScriptData = new List<string>();
            mDataFolder = folder;
            mFileExt = ext;
        }

        /// <summary>
        /// スクリプト編集
        /// </summary>
        public void editor(string path)
        {
            if (!string.IsNullOrEmpty(path))
                load(path);
            int opeCount = 0;
            while (true) {
                string command = "", arg = "";
                Console.Write($"ed {PROMPT}");
                var input = Console.ReadLine().Trim();
                int sp = input.IndexOf(" ");
                if (0 < sp) {
                    command = input.Substring(0, sp).Trim();
                    arg = input.Substring(sp).Trim();
                } else
                    command = input.Trim();

                if (command.Length == 0)
                    continue;
                else if (0 == "list".IndexOf(command)) {
                    list(arg);
                    continue;
                } else if (0 == "add".IndexOf(command)) {
                    mScriptData.Add(arg);
                } else if (0 == "insert".IndexOf(command)) {
                    insert(arg);
                } else if (0 == "remove".IndexOf(command)) {
                    remove(arg);
                } else if (0 == "move".IndexOf(command)) {
                    move(arg);
                } else if (0 == "copy".IndexOf(command)) {
                    copy(arg);
                } else if (0 == "clear".IndexOf(command)) {
                    mScriptData.Clear();
                } else if (0 == "search".IndexOf(command)) {
                    search(arg);
                    continue;
                } else if (0 == "replace".IndexOf(command)) {
                    replace(arg);
                } else if (0 == "tab2space".IndexOf(command)) {
                    tab2space(arg);
                } else if (0 == "editor".IndexOf(command)) {
                    int n = ylib.intParse(arg.Trim());
                    if (0 <= n && n < mScriptData.Count) {
                        mScriptData[n] = lineEdit(">> ", mScriptData[n]);
                        Console.WriteLine();
                    }
                } else if (0 == "execute".IndexOf(command)) {
                    execute(0 < opeCount);
                    continue;
                } else if (0 == "calculate".IndexOf(command)) {
                    calculate(arg);
                    continue;
                } else if (0 == "load".IndexOf(command)) {
                    load(arg);
                    opeCount = 0;
                    continue;
                } else if (0 == "save".IndexOf(command)) {
                    if (0 < opeCount) {
                        if (save(arg))
                            opeCount = 0;
                    }
                    continue;
                } else if (0 == "help".IndexOf(command)) {
                    helpEd(arg);
                    continue;
                } else if (0 == "quit".IndexOf(command)) {
                    if (0 < opeCount)
                        save("");
                    return;
                }
                opeCount++;
            }
        }

        /// <summary>
        /// 行の移動(move 3,10)
        /// </summary>
        /// <param name="arg">移動前,移動後</param>
        private void move(string arg)
        {
            int st = 0, ed = mScriptData.Count;
            string[] no = arg.Split(',');
            if (0 < no.Length)
                st = ylib.intParse(no[0]);
            if (1 < no.Length)
                ed = ylib.intParse(no[1]);
            string tmp = mScriptData[st];
            mScriptData.Insert(ed, tmp);
            if (st < ed)
                mScriptData.RemoveAt(st);
            else
                mScriptData.RemoveAt(st + 1);
        }

        /// <summary>
        /// 行のコピー(copy 3, 10)
        /// </summary>
        /// <param name="arg">コピー前行,コピー後行</param>
        private void copy(string arg)
        {
            int st = 0, ed = mScriptData.Count;
            string[] no = arg.Split(',');
            if (0 < no.Length)
                st = ylib.intParse(no[0]);
            if (1 < no.Length)
                ed = ylib.intParse(no[1]);
            string tmp = mScriptData[st];
            mScriptData.Insert(ed, tmp);
        }

        /// <summary>
        /// 数式の計算
        /// </summary>
        /// <param name="arg">数式</param>
        private void calculate(string arg)
        {
            Console.Write($" {PROMPT}");
            Console.WriteLine(calc.expression(arg));
        }

        /// <summary>
        /// スクリプトファイルの読み込み
        /// </summary>
        /// <param name="arg">ファイルパス</param>
        private void load(string arg)
        {
            if (string.IsNullOrEmpty(arg)) {
                if (!Directory.Exists(mDataFolder))
                    mDataFolder = ".";
                //  ファイル選択
                mScriptPath = ylib.consoleFileSelect(mDataFolder, "*" + mFileExt);
                mScriptData = ylib.loadListData(mScriptPath);
            } else {
                string ext = Path.GetExtension(arg);
                string fname = arg;
                if (string.IsNullOrEmpty(ext))
                    fname += mFileExt;
                else
                    fname = arg.Replace(ext, mFileExt);
                if (Path.GetDirectoryName(fname) == "")
                    fname = Path.Combine(mDataFolder, fname);
                if (File.Exists(fname)) {
                    mScriptPath = Path.GetFullPath(fname);
                    mScriptData = ylib.loadListData(mScriptPath);
                }
            }
        }

        /// <summary>
        /// スクリプトファイルの保存
        /// </summary>
        /// <param name="arg">ファイルパス</param>
        private bool save(string arg) 
        {
            if (arg != null && 0 < arg.Length) {
                string ext = Path.GetExtension(arg);
                string fname = arg;
                if (string.IsNullOrEmpty(ext))
                    fname += mFileExt;
                else
                    fname = arg.Replace(ext, mFileExt);
                mScriptPath = Path.Combine(mDataFolder, fname);
            } else if (mScriptPath == "") {
                Console.WriteLine($"ファイル名が設定されていません");
                return false;
            }
            if (File.Exists(mScriptPath)) {
                Console.Write($"{mScriptPath} を上書きしますか? (y(es)/n(o)?");
                var key = Console.ReadKey();
                if (char.ToLower(key.KeyChar) != 'y') {
                    Console.WriteLine();
                    return false;
                }
            }
            ylib.saveListData(mScriptPath, mScriptData);
            Console.WriteLine();
            return true;
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="arg">検索ワード</param>
        private void search(string arg)
        {
            if (arg == null || arg.Length == 0)
                return;
            for (int i = 0; i < mScriptData.Count; i++) {
                if (0 <= mScriptData[i].IndexOf(arg))
                    Console.WriteLine($"{i.ToString("D4")} {mScriptData[i]}");
            }
        }

        /// <summary>
        /// 置換え
        /// </summary>
        /// <param name="arg">[検索ワード],[置換えワード]</param>
        private void replace(string arg)
        {
            string[] args = arg.Split(',');
            bool contFlag = false;
            var key = new ConsoleKeyInfo();
            ConsoleKeyInfo preKey = new ConsoleKeyInfo();
            if (2 == args.Length) {
                for (int i = 0; i < mScriptData.Count; i++) {
                    if (0 <= mScriptData[i].IndexOf(args[0])) {
                        Console.WriteLine($"{i.ToString("D4")} {mScriptData[i]}");
                        string buf = mScriptData[i].Replace(args[0], args[1]);
                        if (!contFlag) {
                            Console.WriteLine($"{i.ToString("D4")} {buf}");
                            Console.Write("y(es)/n(o)/c(ontinue)/q(uit) :");
                            key = Console.ReadKey();
                            if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                mScriptData[i] = buf;
                            else if (key.KeyChar == 'c' || key.KeyChar == 'C') {
                                contFlag = true;
                                key = preKey;
                                i--;
                            } else if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                                break;
                            preKey = key;
                            Console.WriteLine();
                        } else {
                            if (key.KeyChar == 'y' || key.KeyChar == 'Y')
                                mScriptData[i] = buf;
                            Console.WriteLine($"{i.ToString("D4")} {mScriptData[i]}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 行の挿入
        /// </summary>
        /// <param name="arg">行番号と行内容</param>
        private void insert(string arg)
        {
            arg = arg.Trim();
            int sp = arg.IndexOf(" ");
            if (0 < sp) {
                int n = ylib.intParse(arg.Substring(0, sp).Trim());
                string buf = arg.Substring(sp + 1);
                if (0 <= n && n < mScriptData.Count)
                    mScriptData.Insert(n, buf);
            }
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        private void execute(bool saveOn)
        {
            if (saveOn)
                save("");
            callback();
            //string scriptData = string.Join("\n", mScriptData);
            //Script script = new Script(scriptData);
            //script.execute("main");
        }

        /// <summary>
        /// 行の削除
        /// </summary>
        /// <param name="arg">行番号</param>
        private void remove(string arg)
        {
            string[] no = arg.Split(',');
            if (no.Length == 1) {
                int n = ylib.intParse(no[0]);
                if (0 <= n && n < mScriptData.Count)
                    mScriptData.RemoveAt(n);
            } else if (no.Length == 2) {
                int st = ylib.intParse(no[0]);
                int ed = ylib.intParse(no[1]);
                for (int i = ed; st <= i; i--)
                    mScriptData.RemoveAt(i);
            }
        }

        /// <summary>
        /// リスト表示
        /// </summary>
        /// <param name="arg">[開始行],[終了行]</param>
        private void list(string arg)
        {
            int st = 0, ed = mScriptData.Count;
            string[] no = arg.Split(',');
            if (0 < no.Length)
                st = ylib.intParse(no[0]);
            if (1 < no.Length)
                ed = ylib.intParse(no[1]) + 1;
            ed = Math.Min(ed, mScriptData.Count);
            st = Math.Max(Math.Min(st, ed), 0);
            for (int i = st; i < ed; i++) {
                Console.WriteLine($"{i.ToString("D4")} {mScriptData[i]}");
            }
        }

        /// <summary>
        /// タブをスペースに変換
        /// </summary>
        /// <param name="arg">[開始行],[終了行]</param>
        private void tab2space(string arg)
        {
            int st = 0, ed = mScriptData.Count;
            string[] no = arg.Split(',');
            if (0 < no.Length)
                st = ylib.intParse(no[0]);
            if (1 < no.Length)
                ed = ylib.intParse(no[1]) + 1;
            ed = Math.Min(ed, mScriptData.Count);
            st = Math.Max(Math.Min(st, ed), 0);
            for (int i = st; i < ed; i++) {
                mScriptData[i] = ylib.tab2space(mScriptData[i]);
                Console.WriteLine($"{i.ToString("D4")} {mScriptData[i]}");
            }
        }


        /// <summary>
        /// 行編集
        /// </summary>
        /// <param name="prompt">プロンプト</param>
        /// <param name="buf">編集文字列</param>
        /// <returns>編集後の文字列</returns>
        public string lineEdit(string prompt, string buf)
        {
            string original = buf;
            string dispStr = prompt + buf;
            int promptLen = prompt.Length;
            int displen = ylib.getStrByteCount(dispStr);
            Console.CursorVisible = true;
            (int left, int top) = Console.GetCursorPosition();
            Console.Write(dispStr);
            int sp = buf.Length;
            while (true) {
                var key = Console.ReadKey();
                switch (key.Key) {
                    case ConsoleKey.Escape: return original;
                    case ConsoleKey.Enter: return buf;
                    case ConsoleKey.LeftArrow: sp--; break;
                    case ConsoleKey.RightArrow: sp++; break;
                    case ConsoleKey.UpArrow: break;
                    case ConsoleKey.DownArrow: break;
                    case ConsoleKey.Home: sp = 0; break;
                    case ConsoleKey.End: sp = buf.Length; break;
                    case ConsoleKey.Backspace:
                        if (0 < sp) {
                            sp--;
                            buf = buf.Remove(sp, 1);
                        }
                        break;
                    case ConsoleKey.Delete:
                        if (sp < buf.Length)
                            buf = buf.Remove(sp, 1);
                        break;
                    default:
                        buf = buf.Insert(sp++, key.KeyChar.ToString());
                        break;
                }
                sp = Math.Max(0, Math.Min(sp, buf.Length));
                Console.SetCursorPosition(0, top);
                Console.Write($"{" ".PadLeft(displen)}");      //  前の文字列を消去して表示
                dispStr = prompt + buf;
                Console.SetCursorPosition(0, top);
                Console.Write($"{dispStr}");
                int cp = ylib.getStrByteCount(buf.Substring(0, sp)) + promptLen;    //  カーソル位置を全角文字に対応
                int width = Console.WindowWidth;
                displen = ylib.getStrByteCount(dispStr);
                if (width <= displen && width <= cp) {
                    Console.SetCursorPosition(cp - width, top + 1);
                } else
                    Console.SetCursorPosition(cp, top);
            }
        }

        /// <summary>
        /// 数式関数のヘルプ表示
        /// </summary>
        private void funcHelp()
        {
            foreach (var str in YCalc.mFuncList)
                Console.WriteLine(str);
        }

        /// <summary>
        /// 編集コマンドのヘルプ表示
        /// </summary>
        /// <param name="arg">オプション</param>
        private void helpEd(string arg)
        {
            if (0 < arg.Length && 0 == "calculate".IndexOf(arg.Trim())) {
                funcHelp();
            } else {
                List<string> help = new List<string>() {
                    "--  editor help --",
                    "add [文字列]                 追加",
                    "list [start,end]             行数指定のリスト表示",
                    "insert No [文字列]           No行に挿入",
                    "remove start[,end]           行削除",
                    "move srcLine,destLine        行移動",
                    "copy srcLine,destLine        行コピー",
                    "search [文字列]              検索",
                    "replace [文字列],[文字列]    置換え",
                    "tab2space [開始行],[終了行]  タブをスペースに変換",
                    "clear                        すべて削除",
                    "load [パス]                  ファイルの読込",
                    "save [パス]                  ファイルの保存",
                    "execute                      スクリプトの実行",
                    "calculate [数式]             数式の計算処理",
                    "help                         ヘルプ",
                    "help [calc]                  数式の関数ヘルプ",
                    "quit                         終了",
                };
                foreach (var str in help)
                    Console.WriteLine($"{str}");
            }
        }
    }
}
