using Decklists.Providers;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            Static.Loader.LoadDatabase();                        
            Static.Loader.LoadStaticInfo();
            Static.Database.Instance.SaveToJSON();
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {            
            Decklists.Providers.PkmnCards pkmnCards = new Providers.PkmnCards();
            pkmnCards.ProviderDownloaded += _providerDownloaded;
            pkmnCards.DownloadCollection(Static.Database.Instance.Collections.First());
        }

        void _providerDownloaded( object sender, EventArgs e )
        {
            ProviderBase provider = sender as ProviderBase;
            provider.PersistData();
            Static.Database.Instance.SaveToJSON();
        }
    }
}
