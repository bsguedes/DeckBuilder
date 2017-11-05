using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public abstract class LigaMagicProvider : ProviderBase
    {
        
        protected abstract string LigaMagicProviderRootURL { get; }

        protected override uint UniqueID => throw new NotImplementedException();

        protected override string CategoryTitle => throw new NotImplementedException();

        protected override Uri AssembleURL(Card card)
        {
            return new Uri(string.Format(@"{3}?view=ecom/item&edicaoP={0}&cardID={1}&cardP={2}", card.Collection.Modifiers["collection_id_liga_magic"], card.Index, card.Modifiers["ecom"], LigaMagicProviderRootURL));
        }

        protected override void HandleHtmlCodeForProvider(Card card, string htmlCode)
        {
            int currIndex = 0;
            int index = 0;
            List<float> prices = new List<float>();
            while (index >= 0) {
                try
                {
                    index = htmlCode.IndexOf("<td class='itemPreco hmin30", currIndex);
                    if (index > 0)
                    {
                        int indexPrice = htmlCode.IndexOf(">", index) + 4;
                        int endIndex = htmlCode.IndexOf("<", indexPrice);
                        float price = float.Parse(new String(htmlCode.Skip(indexPrice).Take(endIndex - indexPrice).ToArray()));
                        prices.Add(price);
                        currIndex = endIndex;
                    }
                }
                catch
                {
                    index = htmlCode.IndexOf("<td class='itemPreco hmin30 ' title='Item com desconto", currIndex);
                    if (index > 0)
                    {
                        int indexPrice = htmlCode.IndexOf("<font color='red'>", index) + 21;
                        int endIndex = htmlCode.IndexOf("<", indexPrice);
                        float price = float.Parse(new String(htmlCode.Skip(indexPrice).Take(endIndex - indexPrice).ToArray()));
                        prices.Add(price);
                        currIndex = endIndex;
                    }
                }

            }
            if (prices.Count > 0)
            {
                this.Data.Add(card.UniqueID, prices.Min().ToString());
            }
        }
    }
}
