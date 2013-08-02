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
            // TODO Remember to add Service.Close() call at application exit
            XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(this.GetType());

            WherelessViewModel viewModel = WherelessViewModel.GetWherelessViewModel();
            ServiceController service = new ServiceController()
            {
                RadioOffCallback = viewModel.UpdateRadioOff,
                UpdateCurrentLocationCallback = viewModel.UpdateCurrentLocation
            };
            viewModel.WherelessService = service;
            service.Start();

            // TEST-CODE
            //Thread.Sleep(5000);
            //viewModel.RegisterLocation("Casa Mare");
            //Thread.Sleep(5000);
            //viewModel.ForceUnknown();
            //Thread.Sleep(5000);
            //viewModel.ForceLocation("Casa Mare");
            //Thread.Sleep(5000);

            log.Info("whereless started...");
            InitializeComponent();
        }
    }
}
