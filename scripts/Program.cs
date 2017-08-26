using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scripts
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "-h")
            {
                Console.WriteLine("List of commands:");
                Console.WriteLine("-l start end file     Downloads all index between start and end from LigaMagic and stores it in file.");
            }
            else if (args[0] == "-l" && args.Length >= 4)
            {
                int start = int.Parse(args[1]);
                int end = int.Parse(args[2]);
                string file = args[3];
                var l = new LigaMagicDownloader(start, end, file);
                l.Download();
            }
            else
            {
                Console.WriteLine("Invalid command, type -h for help.");
            }
        }
    }
}
