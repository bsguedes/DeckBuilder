using Decklists.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MainWindow : Window
    {                
        public ObservableCollection<Quotation> FilteredQuotations { get; set; }

        public ObservableCollection<Card> FilteredCards { get; set; }

        public MainWindow()
        {
            Static.Loader.LoadDatabase();                        
            Static.Loader.LoadStaticInfo();
            Static.Database.Instance.SaveToJSON();

            FilteredQuotations = new ObservableCollection<Quotation>();
            FilteredCards = new ObservableCollection<Card>();

            InitializeComponent();
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            foreach( ProviderDescriptor pd in Static.Database.Instance.Providers)
            {
                if (pd.IsCheckedForDownload)
                {
                    ProviderBase provider = (ProviderBase)Activator.CreateInstance(pd.Type);
                    provider.ProviderDownloaded += _providerDownloaded;
                    foreach (Collection c in Static.Database.Instance.Collections)
                    {
                        if (c.IsCheckedForDownload)
                        {
                            provider.DownloadCollection(c);
                        }
                    }
                }
            }
        }
        

        void _providerDownloaded( object sender, EventArgs e )
        {
            ProviderBase provider = sender as ProviderBase;
            provider.PersistData();
            Static.Database.Instance.SaveToJSON();
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
                    }
                }
                FilteredQuotations.Clear();
                foreach (Quotation q in new_quotes.OrderBy(x => x.ProviderID).ThenBy(x => x.Timestamp))
                {
                    FilteredQuotations.Add(q);
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
