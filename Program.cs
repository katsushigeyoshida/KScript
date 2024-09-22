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
                    execute(mScriptData, mScriptFolder);
                } else if ("quit".IndexOf(command) == 0) {
                    break;
                } else if ("help".IndexOf(command) == 0) {
                    helpMain(arg);
                } else if ("editor".IndexOf(command) == 0) {
                    mConEditor.editor(mScriptPath);
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
        /// スクリプトファイルの選択
        /// </summary>
        static void fileSelct()
        {
            //  テストファイル選択
            mScriptPath = ylib.consoleFileSelect(mScriptFolder, "*.sc");
            if (mScriptPath == null || mScriptPath == "") return;
            mScriptFolder = Path.GetDirectoryName(mScriptPath);
            mScriptData = ylib.loadListData(mScriptPath);
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
        static void execute(List<string> scriptData, string scriptFolder)
        {
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