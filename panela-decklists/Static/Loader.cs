using Decklists.Cards;
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
        public static List<Collection> Collections = new List<Collection>();
        public static List<CardItem> CardItems = new List<CardItem>();
        
        
        public static void Load()
        {
            var csv = new List<string[]>(); 
            var lines = System.IO.File.ReadAllLines( @"data.csv" );

            foreach(var collection in StaticData.SupportedCollections){
                Collections.Add ( new Collection ( collection.Item1, collection.Item2, collection.Item3 ));
            }

            string titles = lines.First();
            var sp = titles.Split( ';' );

            foreach ( string line in lines.Skip(1) )
            {
                string[] ss = line.Split(';');
                int index = int.Parse(ss[0]); //number
                string collAbbr = ss[1]; //code
                string name = ss[ 2 ]; //name

                Dictionary<string, string> dict = new Dictionary<string, string>();

                for ( int i = 3; i < ss.Count(); i++ )
                {
                    dict[ sp[ i ] ] = ss[ i ];
                }

                Card card = new Card( index, name, Collections.First( x => x.Abbreviation == collAbbr ) );
                Collections.First( x => x.Abbreviation == collAbbr ).Cards.Add( card );

                CardItems.Add( new CardItem( index, name, collAbbr, dict ) );
            }
        }

        public static void Save()
        {
            List<string> titles = new List<string>();                        
            titles.AddRange( CardItems.SelectMany( ( x, y ) => x.GetProviders() ).Distinct() );

            using ( StreamWriter sw = new StreamWriter( "new_data.csv", false ) )
            {
                sw.WriteLine( string.Join( ";", titles ) );
                foreach ( var cardItem in CardItems )
                {
                    List<string> line = new List<string>();
                    foreach ( var title in titles )
                    {
                        if ( cardItem.HasProvider( title ) )
                        {
                            line.Add( cardItem.GetProviderValue( title ) );
                        }
                        else
                        {
                            line.Add( string.Empty );
                        }
                    }
                    sw.WriteLine( string.Join( ";", line ) );
                }
                
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
