using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public abstract class ProviderBase
    {
        public event EventHandler ProviderDownloaded;

        public ProviderBase()
        {
            this.Data = new Dictionary<Cards.Card, object>();
        }

        protected Dictionary<Cards.Card, object> Data { get; private set; }
        protected abstract string CategoryTitle { get; }
        protected abstract string AssembleURL( Cards.Card card );
        protected abstract void HandleHtmlCodeForProvider( Cards.Card card, string htmlCode );        
        
        BackgroundWorker bw_wait;

        public void DownloadCollection( Cards.Collection collection )
        {
            int expectation = collection.Cards.Count;

            bw_wait = new BackgroundWorker();
            bw_wait.WorkerReportsProgress = true;

            bw_wait.DoWork += bw_wait_DoWork;
            bw_wait.ProgressChanged += bw_wait_ProgressChanged;
            bw_wait.RunWorkerCompleted += bw_wait_RunWorkerCompleted;

            bw_wait.RunWorkerAsync( expectation );

            foreach ( var card in collection.Cards )
            {
                DownloadContentForCard( card );
            }                        
        }

        int _expectations = 0;

        void bw_wait_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            Console.WriteLine( "Done!" );
            if ( ProviderDownloaded != null )
            {
                ProviderDownloaded( this, new EventArgs() );
            }
        }

        void bw_wait_ProgressChanged( object sender, ProgressChangedEventArgs e )
        {
            _expectations--;
        }

        void bw_wait_DoWork( object sender, DoWorkEventArgs e )
        {
            _expectations = ( int )e.Argument;
            Console.WriteLine( "Started" );
            while ( _expectations > 0 ) ;
        }

        public void DownloadContentForCard( Cards.Card card )
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            bw.RunWorkerAsync( card );
        }

        void bw_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            bw_wait.ReportProgress( 0 );
            if ( e.Error != null )
            {
                Console.WriteLine( "Error downloading card {0}", e.Result );
            }
            else
            {
                Console.WriteLine( "Downloaded card {0}", e.Result );
            }            
        }

        void bw_DoWork( object sender, DoWorkEventArgs e )
        {
            Cards.Card card = e.Argument as Cards.Card;
            try
            {

                using ( WebClient client = new WebClient() )
                {
                    string htmlCode = client.DownloadString( AssembleURL( card ) );
                    this.HandleHtmlCodeForProvider( card, htmlCode );
                }
            }
            catch { }
            finally
            {
                e.Result = card.ToString();
            }

        }


        public void PersistData()
        {
            foreach ( var card in this.Data )
            {
                var cardItem = Static.Loader.CardItems.First( x => x.GetProviderValue( "number" ) == card.Key.Index.ToString() && x.GetProviderValue( "code" ) == card.Key.Collection.Abbreviation );
                cardItem.AddProvider( this.CategoryTitle, card.Value.ToString() );
            }
        }        

    }
}
