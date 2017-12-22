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
                new ProviderDescriptor("Bazar de Bagdá", 4, typeof(BazarDeBagda)),
                new ProviderDescriptor("Chuck TCG", 5, typeof(ChuckTCG))
            };
        }
        
        public List<ProviderDescriptor> ProviderList { get; private set; }
    }

    public class DownloadManager
    {
        public event RunWorkerCompletedEventHandler DownloadManagerCompleted;
        public event ProgressChangedEventHandler DownloadManagerProgressChanged;

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
                    DownloadManagerProgressChanged?.Invoke(this, new ProgressChangedEventArgs(current_download * 100 / max_download, null));
                    if(current_download == max_download)
                    {
                        DownloadManagerCompleted?.Invoke(this, new RunWorkerCompletedEventArgs(providers, null, false));
                    }
                };
                p.DownloadCollections(collections);
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
        
        BackgroundWorker bw_download;

        public void DownloadCollections(IEnumerable<Collection> collections)
        {
            var collectionIds = collections.Select(x => x.UniqueID);
            var cardsFromCollection = Static.Database.Instance.Cards.Where(x => collectionIds.Contains( x.CollectionID )).OrderBy(x => x.CollectionID).ThenBy(y => y.Index);
            int expectation = cardsFromCollection.Count();

            bw_download = new BackgroundWorker();

            bw_download.DoWork += Bw_download_DoWork;        
            bw_download.RunWorkerCompleted += Bw_download_RunWorkerCompleted;

            bw_download.RunWorkerAsync( expectation );

            foreach ( var card in cardsFromCollection)
            {
                DownloadContentForCard( card );
            }                        
        }

        int _expectations = 0;
        
        void Bw_download_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            Console.WriteLine( "Done!" );
            ProviderDownloaded?.Invoke( this, new EventArgs() );            
        }

        void Bw_download_DoWork( object sender, DoWorkEventArgs e )
        {
            _expectations = ( int )e.Argument;            
            Console.WriteLine( "Started" );
            while ( _expectations > 0 );
        }

        public void DownloadContentForCard( Card card )
        {
            BackgroundWorker bw_card = new BackgroundWorker();
            bw_card.DoWork += Bw_card_DoWork;
            bw_card.RunWorkerCompleted += Bw_card_RunWorkerCompleted;
            bw_card.RunWorkerAsync( card );
        }

        void Bw_card_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            lock (this)
            {
                _expectations--;
                ProgressChanged?.Invoke(this, null);
            }
            if ( e.Error != null )
            {
                Console.WriteLine( "Error downloading card {0}", e.Result );
            }
            else
            {
                Console.WriteLine( "Downloaded card {0}", e.Result );
            }            
        }

        void Bw_card_DoWork( object sender, DoWorkEventArgs e )
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
