using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using whereless.MutexManager;

using log4net;
using log4net.Config;
using whereless.LocalizationService;
using whereless.ViewModel;


namespace whereless
{
        static class AppStartup
        {
            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main()
            {

                if (!SingleInstance.Start())
                {
                    SingleInstance.ShowFirstInstance();
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                   


                    // TODO Remember to add Service.Close() call at application exit
                    XmlConfigurator.Configure();

                    //ILog log = LogManager.GetLogger(this.GetType());
                    Type type = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
                    ILog log = LogManager.GetLogger(type);

                    WherelessViewModel viewModel = WherelessViewModel.GetInstance();
                    ServiceController service = new ServiceController()
                    {
                        RadioOffCallback = viewModel.UpdateRadioOff,
                        UpdateCurrentLocationCallback = viewModel.UpdateCurrentLocation
                    };
                    viewModel.WherelessService = service;
                   
                    service.Start();

                    ////TEST-CODE
                    //Thread.Sleep(5000);
                    //viewModel.RegisterLocation("Casa Mare");
                    //viewModel.AddActivityToLocation(viewModel.CurrentLocation, "OpenBrowser", "firefox", "www.polito.it", "ExeFile");

                    //Thread.Sleep(5000);
                    //viewModel.ForceUnknown();
                    //Thread.Sleep(5000);
                    //viewModel.ForceLocation("Casa Mare");
                    //Thread.Sleep(5000);
                    // Now move in another unknown location
                    

                    log.Info("whereless started...");


                    var applicationContext = new CustomApplicationContext();
                    Application.Run(applicationContext);



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Program Terminated UnexpectedlyZZ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                SingleInstance.Stop();

            }
        }

}
