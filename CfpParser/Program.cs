using ConfigR;

namespace CfpParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(Config.Global.Get<string>("filePath"), Config.Global.Get<string>("outputPath"), true, Config.Global.Get<string>("existingFolder"));
            parser.ParseFile();
            parser.WriteOutput();
        }
    }
}
