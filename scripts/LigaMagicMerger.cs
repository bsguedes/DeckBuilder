using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scripts
{
    public class LigaMagicMerger
    {
        private string file;
        private string dest_file;

        public LigaMagicMerger(string file, string dest)
        {
            this.file = file;
            this.dest_file = dest;
        }

        internal void Merge()
        {
            string dataPath = @"..\..\..\decklists\data.csv";
            var lines = File.ReadAllLines(dataPath);
            var ecomlist = File.ReadAllLines(file);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<string[]> dest = new List<string[]>();
            foreach (var line in ecomlist)
            {
                string[] se = line.Split(';');
                if (!dict.ContainsKey(se[1]))
                {
                    dict.Add(se[1], se[0]);
                }
            }
            List<string> sh = lines[0].Split(';').ToList();
            int ecomIndex = sh.IndexOf("ecom");
            int nameIndex = sh.IndexOf("name");
            dest.Add(lines[0].Split(','));
            for (int i = 1; i < lines.Length; i++)
            {
                string[] ss = lines[i].Split(';');
                if (string.IsNullOrEmpty(ss[ecomIndex]))
                {
                    ss[ecomIndex] = dict[ss[nameIndex]];
                }
                dest.Add(ss);
            }
            using (StreamWriter sw = new StreamWriter(dest_file, false))
            {
                foreach (string[] s in dest)
                {
                    sw.WriteLine(string.Join(";", s));
                }
            }
        }
    }
}
