// See https://aka.ms/new-console-template for more information
using CoreLib;

namespace KScript
{
    class KScrpt
    {
        static List<string> mScriptData = new List<string>();
        static string mScriptPath = "";
        static string mScriptFolder = "";
        static Script mScript;
        const string PROMPT = ">> ";

        static ConsoleEditor mConEditor;
        static YLib ylib = new YLib();

        static void Main(string[] args)
        {
            YCalc calc = new YCalc();
            Console.Clear();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(abortHandler);

            Console.WriteLine("KScript Test");
            //  既定フォルダの取得
            Properties.Settings.Default.Reload();
            mScriptFolder = Properties.Settings.Default.DataFolder;
            if (!Directory.Exists(mScriptFolder))
                mScriptFolder = ".";

            mConEditor = new ConsoleEditor(mScriptFolder, ".sc");
            mConEditor.callback = execute;
            mScript = new Script();
            mScript.mScriptFolder = mScriptFolder;

            while (true) {
                Console.Write($"main {PROMPT}");
                var input = Console.ReadLine();
                if (input == null)
                    continue;
                string command = "", arg = "";
                int sp = input.IndexOf(" ");
                if (0 < sp) {
                    command = input.Substring(0, sp).Trim();
                    arg = input.Substring(sp).Trim();
                } else
                    command = input.Trim();
                if (string.IsNullOrEmpty(input)) {
                    fileSelct();
                } else if ("list".IndexOf(command) == 0) {
                    listDisp(mScriptData);
                } else if ("load".IndexOf(command) == 0) {
                    fileSelct();
                } else if ("addload".IndexOf(command) == 0) {
                    addFile();
                } else if ("execute".IndexOf(command) == 0) {
                    execute(mScriptData, mScriptFolder, arg);
                } else if ("quit".IndexOf(command) == 0) {
                    break;
                } else if ("help".IndexOf(command) == 0) {
                    helpMain(arg);
                } else if ("editor".IndexOf(command) == 0) {
                    mConEditor.editor(mScriptPath);
                    load(mScriptPath);
                } else if (0 < input.Length) {
                    Console.Write($" {PROMPT}");
                    Console.WriteLine(calc.expression(input.Trim()));
                }
            }

            //  既定フォルダの保存
            mScriptFolder = Path.GetDirectoryName(mScriptPath);
            if (mScriptFolder != null) {
                Properties.Settings.Default.DataFolder = mScriptFolder;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// プログラムの中断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected static void abortHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine($"\n{args.SpecialKey} : Program Abort");
            Environment.Exit(0);
        }

        /// <summary>
        /// スクリプトファイルの選択
        /// </summary>
        static void fileSelct()
        {
            //  テストファイル選択
            mScriptPath = ylib.consoleFileSelect(mScriptFolder, "*.sc");
            if (mScriptPath == null || mScriptPath == "") return;
            load(mScriptPath);
        }

        /// <summary>
        /// ファイルを読み込む
        /// </summary>
        /// <param name="path"></param>
        static void load(string path)
        {
            mScriptFolder = Path.GetDirectoryName(path);
            mScriptData = ylib.loadListData(path);
        }

        /// <summary>
        /// スクリプトファイルの追加
        /// </summary>
        static void addFile()
        {
            //  テストファイル選択
            mScriptPath = ylib.consoleFileSelect(mScriptFolder, "*.sc");
            if (mScriptPath == null || mScriptPath == "") return;
            mScriptData = ylib.loadListData(mScriptPath);
            string code = string.Join("\n", mScriptData);
            mScript.addScript(code);
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        /// <param name="scriptData"></param>
        static void execute(List<string> scriptData, string scriptFolder, string arg = "")
        {
            mScript.mDebug = false;
            mScript.mDebugConsole = false;
            if (0 < arg.Length) {
                string[] args = arg.Split(',');
                for (int i = 0; i < args.Length; i++) {
                    if (0 == "debug".IndexOf(args[i].Trim()))
                        mScript.mDebug = true;
                    else if (0 == "console".IndexOf(args[i].Trim()))
                        mScript.mDebugConsole = true;
                }
            }
            mScript.clear();
            string code = string.Join("\n", scriptData);
            mScript.mScriptFolder = scriptFolder;
            mScript.setScript(code);
            mScript.execute("main");
            Console.WriteLine();
        }

        /// <summary>
        /// スクリプトの表示
        /// </summary>
        /// <param name="scriptData"></param>
        static void listDisp(List<string> scriptData)
        {
            string script = string.Join("\n", scriptData);
            Console.WriteLine(script);
            Console.WriteLine($"{'\n'}");
        }

        /// <summary>
        /// スクリプトの実行
        /// </summary>
        static void execute()
        {
            if (mConEditor != null) {
                mScriptPath = mConEditor.mScriptPath;
                mScriptFolder = Path.GetDirectoryName(mScriptPath);
            }
            mScriptData = ylib.loadListData(mScriptPath);
            execute(mScriptData, mScriptFolder);
        }

        /// <summary>
        /// 数式関数のヘルプ表示
        /// </summary>
        static void funcHelp()
        {
            foreach (var str in YCalc.mFuncList)
                Console.WriteLine(str);
        }

        /// <summary>
        /// mainのヘルプ表示
        /// </summary>
        /// <param name="arg">オプション</param>
        static void helpMain(string arg)
        {
            if (0 < arg.Length && 0 == "calculate".IndexOf(arg.Trim())) {
                funcHelp();
            } else {
                List<string> help = new List<string>() {
                    "-- main help --",
                    "load        : スクリプトファイルの選択",
                    "list        : スクリプトの表示",
                    "execute     : スクリプトの実行",
                    "quit        : 終了",
                    "editor      : スクリプトの編集",
                    "help        : ヘルプ",
                    "help [calc] : 数式の関数ヘルプ",
                    "[数式]      : 数式処理",
                    "[CR]        : スクリプトファイルの選択",
                };
                foreach (var str in help)
                    Console.WriteLine($"{str}");
            }
        }
    }
}