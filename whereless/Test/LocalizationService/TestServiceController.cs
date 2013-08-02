using log4net;
using NUnit.Framework;
using System.Threading;
using whereless.LocalizationService;

namespace whereless.Test.LocalizationService
{
    [TestFixture(Description = "Test for ServiceLocalizer")]
    class TestServiceController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceController));

        [Test(Description = "ServiceController Test")]
        public void ServiceControllerTest()
        {
            Log.Debug("Main Thread started");
            var sc = new ServiceController();
            
            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(16000);
            Log.Debug("Main thread is awake");

            Log.Debug("Pause ServiceController");
            sc.Pause();

            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(8000);
            Log.Debug("Main thread is awake");

            Log.Debug("Restart ServiceController");
            sc.Play();

            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(16000);
            Log.Debug("Main thread is awake");

            Log.Debug("Stop ServiceController");
            sc.Stop();   
            Log.Debug("ServiceLocator terminated");
        }
    }
}
