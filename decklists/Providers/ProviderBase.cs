using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public static class Provider
    {
        public static readonly uint PKMN_CARDS = 1;
        public static readonly uint PLAYGROUND = 2;
        public static readonly uint EPIC = 3;

    }

    public abstract class ProviderBase
    {
        public event EventHandler ProviderDownloaded;

        public ProviderBase()
        {
            this.Data = new Dictionary<uint, string>();
        }

        protected abstract uint UniqueID { get; }

        protected Dictionary<uint, string> Data { get; private set; }
        protected abstract string CategoryTitle { get; }
        protected abstract Uri AssembleURL( Card card );
        protected abstract void HandleHtmlCodeForProvider( Card card, string htmlCode );        
        
        BackgroundWorker bw_wait;

        public void DownloadCollection(Collection collection)
        {
            var cardsFromCollection = Static.Database.Instance.Cards.Where(x => x.CollectionID == collection.UniqueID).OrderBy(y => y.Index);
            int expectation = cardsFromCollection.Count();

            bw_wait = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };

            bw_wait.DoWork += _bw_wait_DoWork;
            bw_wait.ProgressChanged += _bw_wait_ProgressChanged;
            bw_wait.RunWorkerCompleted += _bw_wait_RunWorkerCompleted;

            bw_wait.RunWorkerAsync( expectation );

            foreach ( var card in cardsFromCollection)
            {
                DownloadContentForCard( card );
            }                        
        }

        int _expectations = 0;

        void _bw_wait_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            Console.WriteLine( "Done!" );
            ProviderDownloaded?.Invoke( this, new EventArgs() );            
        }

        void _bw_wait_ProgressChanged( object sender, ProgressChangedEventArgs e )
        {
            _expectations--;
        }

        void _bw_wait_DoWork( object sender, DoWorkEventArgs e )
        {
            _expectations = ( int )e.Argument;
            Console.WriteLine( "Started" );
            while ( _expectations > 0 ) ;
        }

        public void DownloadContentForCard( Card card )
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += _bw_DoWork;
            bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            bw.RunWorkerAsync( card );
        }

        void _bw_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
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

        void _bw_DoWork( object sender, DoWorkEventArgs e )
        {
            Card card = e.Argument as Card;
            try
            {

                using ( WebClient client = new WebClient() )
                {
                    Uri url = AssembleURL(card);
                    string htmlCode = client.DownloadString( url );
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
                Quotation q = new Quotation(card.Key, this.UniqueID, DateTime.UtcNow.ToFileTimeUtc(), card.Value);
                Static.Database.Instance.AddQuotation(q);
            }
        }                
    }
}
