using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CfpParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var existingFolder = @"C:\Users\matskasc\Documents\GitHub\callForPapers.github.io\_data\conferences";
            var parser = new Parser(@"C:\temp\conferences.txt", @"C:\temp\confOutput", true, existingFolder);
            parser.ParseFile();
            parser.WriteOutput();
        }
    }
}
