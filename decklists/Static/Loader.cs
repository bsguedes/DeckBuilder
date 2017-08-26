using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                int index = int.Parse(ss[0]); //number
                string collAbbr = ss[1]; //code
                string name = ss[2]; //name

                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 3; i < ss.Count(); i++)
                {
                    dict[sp[i]] = ss[i];
                }

                Database.Instance.AddCard(new Card(index, name, Database.Instance.Collections.First(x => x.Abbreviation == collAbbr).UniqueID, dict));                
            }
        }        
    }

}
