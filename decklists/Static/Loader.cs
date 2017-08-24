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
            // check if there are new collections
            foreach (var collection in StaticData.SupportedCollections)
            {
                Database.Instance.AddCollection(new Collection(collection.Item1, collection.Item2, collection.Item3));                
            }

            // check if there are new cards
            var csv = new List<string[]>();
            var lines = System.IO.File.ReadAllLines(@"data.csv");
            
            foreach (string line in lines.Skip(1))
            {
                string[] ss = line.Split(';');
                int index = int.Parse(ss[0]); //number
                string collAbbr = ss[1]; //code
                string name = ss[2]; //name

                Database.Instance.AddCard(new Card(index, name, Database.Instance.Collections.First(x => x.Abbreviation == collAbbr).UniqueID, null));                
            }
        }        
    }

    public class CardItem
    {
        private Dictionary<string, string> Providers { get; set; }

        public CardItem( int number, string name, string collectionCode, Dictionary<string, string> dict )
        {
            this.Providers = new Dictionary<string, string>();
            AddProvider( "number", number.ToString() );
            AddProvider( "code", collectionCode );
            AddProvider( "name", name );
            foreach ( var item in dict )
            {
                AddProvider( item.Key, item.Value );
            }
        }

        public void AddProvider( string provider, string value )
        {
            this.Providers.Add( provider, value );
        }

        public IEnumerable<string> GetProviders()
        {
            return Providers.Keys.Distinct();
        }

        public bool HasProvider( string title )
        {
            return this.Providers.ContainsKey( title );
        }

        public string GetProviderValue( string title )
        {
            return this.Providers[ title ];
        }
    }

}
