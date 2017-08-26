using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public abstract class LigaMagicProvider : ProviderBase
    {
        public LigaMagicProvider()
        {
        }

        protected override uint UniqueID => throw new NotImplementedException();

        protected override string CategoryTitle => throw new NotImplementedException();

        protected override Uri AssembleURL(Card card)
        {
            return new Uri(string.Format(@"https://{{0}}/?view=ecom/item&edicaoP={0}&cardID={1}&cardP={2})", card.Collection.Modifiers["collection_id_liga_magic"], card.Index, card.Modifiers["card_id_liga_magic"]));
        }

        protected override void HandleHtmlCodeForProvider(Card card, string htmlCode)
        {
            throw new NotImplementedException();
        }
    }
}
