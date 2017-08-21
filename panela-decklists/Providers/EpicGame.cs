using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class EpicGame : ProviderBase
    {
        string BASE_URL = @"http://www.epicgame.com.br/?view=ecom%2Fitens&id=461&busca={0}+%28%23{1}%2F{2}%29";

        protected override string CategoryTitle
        {
            get { return "epic"; }
        }

        protected override uint UniqueID => Provider.EPIC;

        protected override Uri AssembleURL( Card card )
        {
            return new Uri( string.Format( BASE_URL, card.Name, card.Index, card.Collection.MaxValue ) );
        }

        protected override void HandleHtmlCodeForProvider( Card card, string htmlCode )
        {
            int start = htmlCode.IndexOf( "R$" );
            int comma = htmlCode.IndexOf( ",", start );
            string cost = htmlCode.Substring( start, comma + 3 - start );
            this.Data.Add( card.UniqueID, cost );
            Console.WriteLine( "{0}: {1}", card, cost );
        }
    }
}
