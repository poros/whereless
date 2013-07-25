using log4net;
using log4net.Config;
using System.Windows;

namespace whereless
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(this.GetType());
            log.Info("whereless started...");
            InitializeComponent();
        }
    }
}
