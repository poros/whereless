using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using whereless.WiFi;

namespace whereless.Test.WiFi
{
    using NUnit.Framework;

    [TestFixture(Description = "Test for Example")]
    class TestWiFiSensor
    {
        
        [Test(Description = "NativeWiFi example test")]
        public void WiFiSensorTest()
        {
            var stopThread = new AutoResetEvent(false);
            var pauseThread = new AutoResetEvent(false);
            var playThread = new AutoResetEvent(false);
            var wifiSensor = new WiFiSensor(stopThread, pauseThread, playThread);
            var wifiDelegate = new ThreadStart(wifiSensor.ExampleMain);
            var wifiThread = new Thread(wifiDelegate);
            wifiThread.Start();
            
            Console.WriteLine("Main Thread go to sleep");
            Thread.Sleep(16000);
            Console.WriteLine("Main thread is awake");

            Console.WriteLine("Pause WiFiSensor");
            pauseThread.Set();

            Console.WriteLine("Main Thread go to sleep");
            Thread.Sleep(8000);
            Console.WriteLine("Main thread is awake");

            Console.WriteLine("Restart WiFiSensor");
            playThread.Set();

            Console.WriteLine("Main Thread go to sleep");
            Thread.Sleep(16000);
            Console.WriteLine("Main thread is awake");

            Console.WriteLine("Stop WiFiSensor");
            stopThread.Set();

            Console.WriteLine("Wait for WiFiSensor");
            wifiThread.Join();
            Console.WriteLine("WiFiSensor has spontaneously terminated");
        }
    }
}
