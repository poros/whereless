using System.Windows.Controls;
using FluentNHibernate.Conventions.AcceptanceCriteria;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            if (((Button)sender).Name.Equals("ButtonPause") == true || ((Button)sender).Name.Equals("ButtonPauseB") == true || ((Button)sender).Name.Equals("ButtonPauseC") == true)
            {
                viewModel.PauseService();
            }
            else
            {
                if (((Button)sender).Name.Equals("ButtonPlay") == true)
                {
                    viewModel.PlayService();
                }
            }
            
        }
    }
}
