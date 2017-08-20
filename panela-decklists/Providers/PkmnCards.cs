using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class PkmnCards : ProviderBase
    {
        string BASE_URL = "http://pkmncards.com/api/?q=";

        protected override string AssembleURL( Cards.Card card )
        {
            return string.Format( "{0}{1} {2} {3}", BASE_URL, card.Name, card.Collection.Name, card.Index );
        }

        protected override void HandleHtmlCodeForProvider( Cards.Card card, string htmlCode )
        {
            int start = htmlCode.IndexOf( "http://pkmncards.com" );
            int end = htmlCode.IndexOf( ".jpg" ) + 4;
            string imgURL = htmlCode.Substring( start, end - start );
            string fileName = card.Index.ToString( "D3" ) + ".jpg";
            using ( WebClient client = new WebClient() )
            {
                client.DownloadFile( imgURL, fileName );
            }
            this.Data.Add( card, fileName );
        }

        protected override string CategoryTitle
        {
            get { return "pkmncards"; }
        }
    }
}
