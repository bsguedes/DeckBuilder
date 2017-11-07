using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class PkmnCards : ProviderBase
    {
        string BASE_URL = "http://pkmncards.com/api/?q=";        

        protected override Uri AssembleURL( Card card )
        {
            return new Uri( string.Format( "{0}{1} {2} {3}", BASE_URL, card.Name, card.Collection.Name, card.Index ).Replace(" ", "+") );
        }

        protected override void HandleHtmlCodeForProvider( Card card, string htmlCode )
        {
            int start = htmlCode.IndexOf( "https://pkmncards.com" );
            int end = htmlCode.IndexOf( ".jpg" ) + 4;
            string imgURL = htmlCode.Substring( start, end - start );
            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
            }
            if (!Directory.Exists(string.Format("Images/{0}", card.Collection.Abbreviation)))
            {
                Directory.CreateDirectory(string.Format("Images/{0}", card.Collection.Abbreviation));
            }
            string fileName = "Images/" + card.Collection.Abbreviation + "/" + card.Index.ToString( "D3" ) + ".jpg";
            if (!File.Exists(fileName))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(imgURL, fileName);
                }
                this.Data.Add(card.UniqueID, fileName);
            }
        }

        protected override string CategoryTitle
        {
            get { return "pkmncards"; }
        }        
    }
}
