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
