using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class MestrePokemon : ProviderBase
    {
        protected override string CategoryTitle
        {
            get { return "mestre"; }
        }

        protected override string AssembleURL( Cards.Card card )
        {
            throw new NotImplementedException();
        }

        protected override void HandleHtmlCodeForProvider( Cards.Card card, string htmlCode )
        {
            throw new NotImplementedException();
        }
    }
}
