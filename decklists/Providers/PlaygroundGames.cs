using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class PlaygroundGames : LigaMagicProvider
    {       
        protected override string LigaMagicProviderRootURL => @"https://www.playgroundgames.com.br/";

        protected override string CategoryTitle
        {
            get { return "shaman"; }
        }
    }
}
