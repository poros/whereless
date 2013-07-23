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
            var continueThread = new ManualResetEvent(true);
            var wifiSensor = new WiFiSensor(continueThread);
            var wifiDelegate = new ThreadStart(wifiSensor.ExampleMain);
            var wifiThread = new Thread(wifiDelegate);
            wifiThread.Start();
            
            Console.WriteLine("Main Thread go to sleep");
            Thread.Sleep(8000);
            Console.WriteLine("Main thread is awake");

            Console.WriteLine("Pause WiFiSensor");
            continueThread.Reset();

            Console.WriteLine("Main Thread go to sleep");
            Thread.Sleep(8000);
            Console.WriteLine("Main thread is awake");

            Console.WriteLine("Restart WiFiSensor");
            continueThread.Set();

            Console.WriteLine("Main Thread go to sleep");
            Thread.Sleep(8000);
            Console.WriteLine("Main thread is awake");

            Console.WriteLine("Stop WiFiSensor");
            wifiSensor.StopThread = true;

            Console.WriteLine("Wait for WiFiSensor");
            wifiThread.Join();
        }
    }
}
