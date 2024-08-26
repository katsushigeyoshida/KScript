// See https://aka.ms/new-console-template for more information
using CoreLib;

namespace KScript
{
    class KScrpt
    {
        static YLib ylib = new YLib();

        static void Main(string[] args)
        {
            Console.WriteLine("KScript Test");
            const string PROMPT = ">> ";
            while (true) {
                scriptTest();
                Console.Write($"{"\n"} {PROMPT}");
                var input = Console.ReadLine();
                if (0 < input.Length && input[0] == 'q') return;
                //if (string.IsNullOrEmpty(input)) return;
            }
        }

        //  Script Test
        static void scriptTest()
        {
            //  既定フォルダの取得
            KScript.Properties.Settings.Default.Reload();
            string dataFolder = KScript.Properties.Settings.Default.DataFolder;
            if (!Directory.Exists(dataFolder))
                dataFolder = ".";

            //  テストファイル選択
            string scriptFile = ylib.consoleFileSelect(dataFolder, "*.sc");
            string scriptData = ylib.loadTextFile(scriptFile);
            if (scriptData == null || scriptData.Length <= 0) return;

            Console.WriteLine(scriptData);
            Console.WriteLine($"{'\n'}");

            Script script = new Script(scriptData);
            script.execute("main");

            //KParse parse = new KParse(scriptData);
            //parse.execute("main");

            //  既定フォルダの保存
            dataFolder = Path.GetDirectoryName(scriptFile);
            KScript.Properties.Settings.Default.DataFolder = dataFolder;
            KScript.Properties.Settings.Default.Save();
        }
    }
}