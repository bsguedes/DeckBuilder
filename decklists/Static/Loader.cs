using System.Collections.Generic;
using System.Linq;

namespace Decklists.Static
{
    public static class Loader
    {
        internal static void LoadDatabase()
        {
            Database.Instance.ReloadFromJSON();
        }

        internal static void LoadStaticInfo()
        {
            // check if there are new cards
            var csv = new List<string[]>();
            var lines = System.IO.File.ReadAllLines(@"data.csv");

            string titles = lines.First();
            var sp = titles.Split(';');

            foreach (string line in lines.Skip(1))
            {
                string[] ss = line.Split(';');
                int index = ParseIndex(ss[0]); //number
                string str_index = ss[0];
                string collAbbr = ss[1]; //code
                string name = ss[2]; //name

                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 3; i < ss.Count(); i++)
                {
                    dict[sp[i]] = ss[i];
                }

                Database.Instance.AddCard(new Card(index, str_index, name, Database.Instance.Collections.First(x => x.Abbreviation == collAbbr).UniqueID, dict));
            }
        }

        private static int ParseIndex(string s)
        {
            if (s.StartsWith("RC"))
            {
                return int.Parse(new string(s.Skip(2).ToArray()));
            }
            return int.Parse(s);
        }
    }

}
