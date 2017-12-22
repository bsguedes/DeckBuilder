namespace Decklists.Providers
{
    public class ChuckTCG : LigaMagicProvider
    {
        protected override string CategoryTitle
        {
            get { return "chuck"; }
        }

        protected override string LigaMagicProviderRootURL => @"https://www.chucktcg.com.br/";
    }
}
