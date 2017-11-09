using Decklists.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Decklists
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {                
        public ObservableCollection<Quotation> FilteredQuotations { get; set; }

        public ObservableCollection<Card> FilteredCards { get; set; }

        public bool OnlyRecentValue { get; set; }

        private int _percentage;      
        public int DownloadProgressPercentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                PropertyChanged(this, new PropertyChangedEventArgs("DownloadProgressPercentage"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            Static.Loader.LoadDatabase();                        
            Static.Loader.LoadStaticInfo();
            Static.Database.Instance.SaveToJSON();

            FilteredQuotations = new ObservableCollection<Quotation>();
            FilteredCards = new ObservableCollection<Card>();

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btnDownload.IsEnabled = false;
            DownloadProgressPercentage = 0;

            List<ProviderBase> providers = new List<ProviderBase>();
            List<Collection> collections = new List<Collection>();

            foreach (ProviderDescriptor pd in Static.Database.Instance.Providers)
            {
                if (pd.IsCheckedForDownload)
                {
                    ProviderBase provider = (ProviderBase)Activator.CreateInstance(pd.Type);
                    providers.Add(provider);
                }
            }
            foreach (Collection c in Static.Database.Instance.Collections)
            {
                if (c.IsCheckedForDownload)
                {
                    collections.Add(c);
                }
            }

            DownloadManager dm = new DownloadManager();
            dm.DownloadManagerProgressChanged += Dm_ProgressChanged;
            dm.DownloadManagerCompleted += Dm_DownloadCompleted;
            dm.Download(providers, collections);
        }

        private void Dm_DownloadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach( ProviderBase p in (e.Result as IEnumerable<ProviderBase>))
            {
                p.PersistData();
            }
            DownloadProgressPercentage = 100;
            Static.Database.Instance.SaveToJSON();
            btnDownload.IsEnabled = true;
        }

        private void Dm_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            DownloadProgressPercentage = e.ProgressPercentage;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is Card)
            {
                uint cid = (e.AddedItems[0] as Card).UniqueID;
                List<Quotation> new_quotes = new List<Quotation>();
                foreach (Quotation q in Static.Database.Instance.Quotations)
                {
                    if (q.CardID == cid && q.ProviderID != Static.Database.Instance.Providers.First(x => typeof(PkmnCards) == x.Type).ID) 
                    {
                        new_quotes.Add(q);
                        if ( OnlyRecentValue )
                        {
                            Quotation max = new_quotes.Where(x => x.ProviderID == q.ProviderID).OrderByDescending(y => y.Timestamp).First();
                            new_quotes.RemoveAll(x => x.ProviderID == max.ProviderID && x.Timestamp < max.Timestamp);
                        }                        
                    }
                }
                FilteredQuotations.Clear();
                foreach (Quotation q in new_quotes.OrderBy(x => float.Parse(x.Value)).ThenBy(x => x.ProviderID).ThenByDescending(x => x.Timestamp))
                {
                    FilteredQuotations.Add(q);
                }
                string dir = string.Format("Images/{0}", (e.AddedItems[0] as Card).Collection.Abbreviation);
                if (Directory.Exists(dir))
                {
                    string path = System.IO.Path.Combine(Environment.CurrentDirectory, dir, string.Format("{0:D3}.jpg", (e.AddedItems[0] as Card).Index));
                    Uri uri = new Uri(path);
                    if (File.Exists(path))
                    {
                        try
                        {
                            BitmapImage bitmap = new BitmapImage(uri);
                            cardImage.Source = bitmap;
                        }
                        catch
                        {
                            cardImage.Source = null;
                        }
                    }
                    else
                    {
                        cardImage.Source = null;
                    }
                }
                else
                {
                    cardImage.Source = null;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is Collection)
            {
                uint cid = (e.AddedItems[0] as Collection).UniqueID;
                FilteredCards.Clear();
                foreach (Card c in Static.Database.Instance.Cards)
                {
                    if (c.CollectionID == cid)
                    {
                        FilteredCards.Add(c);
                    }
                }
            }
        }
    }
}
