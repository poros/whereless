using log4net;
using NUnit.Framework;
using System.Threading;
using whereless.LocalizationService;
using whereless.LocalizationService.WiFi;

namespace whereless.Test.LocalizationService.WiFi
{
    [TestFixture(Description = "Test for LocationLocalizer")]
    class TestWiFiSensor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestWiFiSensor));

        [Test(Description = "WiFiSensor Main Loop + Control Thread Test")]
        public void WiFiSensorTest()
        {
            var stopThread = new AutoResetEvent(false);
            var pauseThread = new AutoResetEvent(false);
            var playThread = new AutoResetEvent(false);
            var wifiSensor = new WiFiSensor(stopThread: stopThread, pauseThread: pauseThread,
                playThread: playThread, output: new SensorToLocalizer<SensorOutput>());
            var wifiDelegate = new ThreadStart(wifiSensor.WiFiSensorLoop);
            var wifiThread = new Thread(wifiDelegate);
            wifiThread.Start();
            
            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(16000);
            Log.Debug("Main thread is awake");

            Log.Debug("Pause WiFiSensor");
            pauseThread.Set();

            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(8000);
            Log.Debug("Main thread is awake");

            Log.Debug("Restart WiFiSensor");
            playThread.Set();

            Log.Debug("Main Thread go to sleep");
            Thread.Sleep(16000);
            Log.Debug("Main thread is awake");

            Log.Debug("Stop WiFiSensor");
            stopThread.Set();

            Log.Debug("Wait for WiFiSensor");
            wifiThread.Join();
            Log.Debug("WiFiSensor has spontaneously terminated");
        }
    }
}
