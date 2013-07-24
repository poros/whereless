using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using whereless.WiFi;

namespace whereless.Test.WiFi
{
    using NUnit.Framework;

    [TestFixture(Description = "Test for Example")]
    class TestWiFiSensor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestWiFiSensor));

        [Test(Description = "NativeWiFi example test")]
        public void WiFiSensorTest()
        {
            var stopThread = new AutoResetEvent(false);
            var pauseThread = new AutoResetEvent(false);
            var playThread = new AutoResetEvent(false);
            var wifiSensor = new WiFiSensor(stopThread: stopThread, pauseThread: pauseThread,
                playThread: playThread, output: new LossyProducerConsumerElement<SensorOutput>());
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
