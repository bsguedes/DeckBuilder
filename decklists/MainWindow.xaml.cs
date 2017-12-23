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

        private void StartDownload_Click(object sender, RoutedEventArgs e)
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

            if (providers.Count == 0 || collections.Count == 0)
            {
                btnDownload.IsEnabled = true;
            }
            else
            {
                DownloadManager dm = new DownloadManager();
                dm.DownloadManagerProgressChanged += Dm_ProgressChanged;
                dm.DownloadManagerCompleted += Dm_DownloadCompleted;
                dm.Download(providers, collections);
            }
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
                Card card = (e.AddedItems[0] as Card);

                FilterQuotations(card);
                UpdateCardPicture(card);                
            }
        }

        private void FilterQuotations(Card card)
        {
            uint cid = card.UniqueID;
            Dictionary<uint, List<Quotation>> all_quotes = new Dictionary<uint, List<Quotation>>();
            List<Quotation> new_quotes = new List<Quotation>();
            foreach (Quotation q in Static.Database.Instance.Quotations)
            {
                if (q.CardID == cid && q.ProviderID != Static.Database.Instance.Providers.First(x => typeof(PkmnCards) == x.Type).ID)
                {
                    new_quotes.Add(q);
                    if (!all_quotes.ContainsKey(q.ProviderID))
                    {
                        all_quotes.Add(q.ProviderID, new List<Quotation>());
                    }
                    all_quotes[q.ProviderID].Add(q);

                    if (OnlyRecentValue)
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

            PlotChart(card, all_quotes);
        }

        private void PlotChart(Card card, Dictionary<uint, List<Quotation>> all_quotes)
        {
            OxyPlot.Wpf.PlotView plotView = new OxyPlot.Wpf.PlotView();

            OxyPlot.PlotModel plot = new OxyPlot.PlotModel()
            {
                LegendPlacement = OxyPlot.LegendPlacement.Outside,
                LegendPosition = OxyPlot.LegendPosition.BottomCenter,
                LegendOrientation = OxyPlot.LegendOrientation.Horizontal
                
            };
            plot.Axes.Add(new OxyPlot.Axes.DateTimeAxis());
            plot.Axes.Add(new OxyPlot.Axes.LinearAxis() { Minimum = 0 });                        
            List<DateTime> dates = new List<DateTime>();
            List<double> values = new List<double>();
            foreach(var provider in all_quotes)
            {
                OxyPlot.Series.LineSeries ls = new OxyPlot.Series.LineSeries()
                {
                    Title = Static.Database.Instance.Providers.First(x => x.ID == provider.Key).Name,
                    CanTrackerInterpolatePoints = false,
                    MarkerSize = 2,
                    MarkerType = OxyPlot.MarkerType.Circle,
                    TrackerFormatString = "{0}\n{2:dd/MM/yyyy}\nR$ {4:F2}"
                };
                foreach (Quotation q in provider.Value)
                {
                    DateTime dt = DateTime.FromFileTimeUtc(q.Timestamp);
                    double vl = double.Parse(q.Value);
                    dates.Add(dt);
                    values.Add(vl);
                    ls.Points.Add(OxyPlot.Axes.DateTimeAxis.CreateDataPoint(dt,vl));
                }
                plot.Series.Add(ls);                
            }

            plot.Axes[0].Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(dates.Min().AddDays(-1));
            plot.Axes[0].Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(dates.Max().AddDays(1));
            plot.Axes[1].Maximum = values.Max() * 1.1;

            plotView.Model = plot;
            plotGrid.Children.Clear();
            plotGrid.Children.Add(plotView);
        }

        private void UpdateCardPicture(Card card)
        {
            string dir = string.Format("Images/{0}", card.Collection.Abbreviation);
            if (Directory.Exists(dir))
            {
                string path = System.IO.Path.Combine(Environment.CurrentDirectory, dir, string.Format("{0:D3}.jpg", card.Index));
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
