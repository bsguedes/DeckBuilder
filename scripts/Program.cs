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
            if (args.Length == 0 || args[0] == "-h")
            {
                Console.WriteLine("List of commands:");
                Console.WriteLine("-l start end file            Downloads all index between start and end from LigaMagic and stores it in file.");
                Console.WriteLine("-m data_file dest_file       Tries to match indexes between a ligamagic index list and the datafile.");
            }
            else if (args[0] == "-l" && args.Length >= 4)
            {
                int start = int.Parse(args[1]);
                int end = int.Parse(args[2]);
                string file = args[3];
                var l = new LigaMagicDownloader(start, end, file);
                l.Download();
            }
            else if (args[0] == "-m" && args.Length >= 2)
            {
                string file = args[1];
                string dest = args[2];
                var m = new LigaMagicMerger(file, dest);
                m.Merge();
            }
            else
            {
                Console.WriteLine("Invalid command, type -h for help.");
            }
        }
    }
}
