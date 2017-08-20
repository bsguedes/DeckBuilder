using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class ShamanGames : ProviderBase
    {
        string BASE_URL = "https://www.shamangamesba.com.br/loja?keyword={0}+{1}%2F{2}&search=true&view=category&option=com_virtuemart&virtuemart_category_id=0";

        protected override string AssembleURL( Cards.Card card )
        {
            return string.Format( BASE_URL, card.Name.Replace("'s", ""), card.Index, card.Collection.MaxValue );
        }

        protected override void HandleHtmlCodeForProvider( Cards.Card card, string htmlCode )
        {
            int start = htmlCode.IndexOf( "R$" );
            int comma = htmlCode.IndexOf( ",", start );
            string cost = htmlCode.Substring( start, comma + 3 - start );
            this.Data.Add( card, cost );
            Console.WriteLine( "{0}: {1}", card, cost );
        }

        protected override string CategoryTitle
        {
            get { return "shaman"; }
        }
    }
}
