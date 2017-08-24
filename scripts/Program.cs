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
            var l = new LigaMagicDownloader();
            l.Download();
            Console.ReadLine();            
        }
    }
}
