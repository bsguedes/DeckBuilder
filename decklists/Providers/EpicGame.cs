using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class EpicGame : LigaMagicProvider
    {        
        protected override string CategoryTitle
        {
            get { return "epic"; }
        }

        protected override uint UniqueID => Provider.EPIC;

        protected override string LigaMagicProviderRootURL => @"http://www.epicgame.com.br/";
    }
}
