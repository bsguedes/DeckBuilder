using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Decklists.Providers
{
    public class ProviderDescriptor
    {
        public string Name { get; }
        public uint ID { get; }
        public Type Type { get; }

        public bool IsCheckedForDownload { get; set; }

        public ProviderDescriptor(string name, uint id, Type type)
        {
            this.Name = name;
            this.ID = id;
            this.Type = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Provider
    {
        public Provider()
        {
            this.ProviderList = new List<ProviderDescriptor>
            {
                new ProviderDescriptor("PkmnCards", 1, typeof(PkmnCards)),
                new ProviderDescriptor("Playground Games", 2, typeof(PlaygroundGames)),
                new ProviderDescriptor("Epic Games", 3, typeof(EpicGame)),
                new ProviderDescriptor("Bazar de Bagdá", 4, typeof(BazarDeBagda))
            };
        }
        
        public List<ProviderDescriptor> ProviderList { get; private set; }
    }

    public class DownloadManager
    {
        public event RunWorkerCompletedEventHandler DownloadCompleted;
        public event ProgressChangedEventHandler ProgressChanged;

        private int max_download = 0;
        private int current_download = 0;

        public void Download(IEnumerable<ProviderBase> providers, IEnumerable<Collection> collections)
        {
            max_download = providers.Count() * collections.Sum(c => Static.Database.Instance.Cards.Where(x => x.CollectionID == c.UniqueID).Count());
            foreach (ProviderBase p in providers)
            {
                p.ProgressChanged += (sender, e) =>
                {
                    current_download++;
                    ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(current_download * 100 / max_download, null));
                    if(current_download == max_download)
                    {
                        DownloadCompleted?.Invoke(this, new RunWorkerCompletedEventArgs(providers, null, false));
                    }
                };                
                foreach (Collection c in collections)
                {                    
                    p.DownloadCollection(c);
                }
            }
        }
    }

    public abstract class ProviderBase
    {
        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler ProviderDownloaded;

        public ProviderBase()
        {
            this.Data = new Dictionary<uint, string>();
        }

        protected uint UniqueID => Static.Database.Instance.Providers.First(x => x.Type == this.GetType()).ID;

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
                WorkerReportsProgress = true, WorkerSupportsCancellation = true
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
            ProgressChanged?.Invoke(this, null );
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
            try { bw_wait.ReportProgress(0); } catch { }
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
