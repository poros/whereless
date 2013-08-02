using log4net;
using NUnit.Framework;
using System.Threading;
using whereless.LocalizationService;
using whereless.LocalizationService.Localizer;
using whereless.LocalizationService.WiFi;

namespace whereless.Test.LocalizationService.Localizer
{
    [TestFixture(Description = "Test for LocationLocalizer")]
    class TestLocationLocalizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestLocationLocalizer));

        [Test(Description = "LocationLocalizer Main Loop + Control Thread Test")]
        public void LocationLocalizerTest()
        {
            var stopThread = new AutoResetEvent(false);
            var pauseThread = new AutoResetEvent(false);
            var playThread = new AutoResetEvent(false);
            var localizer = new LocationLocalizer(stopThread: stopThread, pauseThread: pauseThread,
                playThread: playThread, input: new SensorToLocalizer<SensorOutput>());
            var localizerDelegate = new ThreadStart(localizer.LocationLocalizerLoop);
            var localizerThread = new Thread(localizerDelegate);
            localizerThread.Start();
            
            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(8000);
            Log.Debug("Main thread is awake");

            Log.Debug("Pause LocationLocalizer");
            pauseThread.Set();

            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(8000);
            Log.Debug("Main thread is awake");

            Log.Debug("Restart LocationLocalizer");
            playThread.Set();

            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(8000);
            Log.Debug("Main thread is awake");

            Log.Debug("Stop LocationLocalizer");
            stopThread.Set();

            Log.Debug("Wait for LocationLocalizer");
            localizerThread.Join();
            Log.Debug("LocationLocalizer has spontaneously terminated");
        }
    }
}
