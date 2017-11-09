using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class BazarDeBagda : LigaMagicProvider
    {        
        protected override string CategoryTitle
        {
            get { return "bazar"; }
        }        

        protected override string LigaMagicProviderRootURL => @"https://www.bazardebagda.com.br/";
    }
}
