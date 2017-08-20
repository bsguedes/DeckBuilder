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
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            Static.Loader.Load();
            Decklists.Providers.PkmnCards pkmnCards = new Providers.PkmnCards();
            
          //  Decklists.Providers.ShamanGames shaman = new Providers.ShamanGames();
          //  shaman.ProviderDownloaded += providerDownloaded;

            Decklists.Providers.EpicGame eg = new Decklists.Providers.EpicGame();
            eg.ProviderDownloaded += providerDownloaded;
            eg.DownloadCollection( Static.Loader.Collections.First() );
        }

        void providerDownloaded( object sender, EventArgs e )
        {
            ProviderBase provider = sender as ProviderBase;
            provider.PersistData();
            Static.Loader.Save();
        }
    }
}
