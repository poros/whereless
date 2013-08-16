using log4net;
using log4net.Config;
using System.Windows;
using whereless.LocalizationService;
using whereless.ViewModel;
using System.Threading;

namespace whereless
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource wherelessViewModelViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("wherelessViewModelViewSource")));
            // Caricare i dati impostando la proprietà CollectionViewSource.Source:
            // wherelessViewModelViewSource.Source = [origine dati generica]
        }
    }
}
