// See https://aka.ms/new-console-template for more information
using CoreLib;
using KScript;

YLib ylib = new YLib();

Console.WriteLine("KScript Test");
const string PROMPT = ">> ";
while (true) {
    scriptTest();
    Console.Write($"{"\n"} {PROMPT}");
    var input = Console.ReadLine();
    if (input == "q") return;
    //if (string.IsNullOrEmpty(input)) return;

}

void scriptTest()
{
    //  Script Test
    string scriptFile = ylib.consoleFileSelect("..\\..\\..\\TestData", "*.sc");
    string scriptData = ylib.loadTextFile(scriptFile);

    Console.WriteLine(scriptData);
    Console.WriteLine($"{'\n'}");

    KParse parse = new KParse(scriptData);
    //parse.mTokenList.ForEach(p => Console.Write($"{p.mValue}[{p.mType}] "));
    //Console.WriteLine($"{'\n'}");
    parse.execute("main");
}
