using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using NativeWifi;
using System;
using System.Text;

namespace whereless.WiFi
{
    public class WiFiSensor
    {
        // A windows logo compliant wlan interface needs to complete the scan in at most 4 seconds
        // 300ms given for thread spawning (just to be sure)
        private readonly int waitTime = 4300;
        private readonly WaitHandle[] _threadControls = new AutoResetEvent[2];
        private readonly WaitHandle _play;
        private WlanClient client;

        // Converts a 802.11 SSID to a string.
        static string GetStringForSsid(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        public WiFiSensor(WaitHandle stopThread, WaitHandle pauseThread, WaitHandle playThread)
        {
            client = new WlanClient();
            _threadControls[0] = stopThread;
            _threadControls[1] = pauseThread;
            _play = playThread;
        }

        public void ExampleMain()
        {
            Console.WriteLine("SCAN");
            var networks = GetNetworksAndScan();
            foreach (var network in networks)
            {
                Console.WriteLine("Network SSID: {0} SignalQuality: {1}",
                                  network.Ssid, network.SignalQuality);
            }
            while (true)
            {
                int handle = WaitHandle.WaitAny(_threadControls, waitTime);
                if (handle == 0)
                {
                    break;

                }
                else if (handle == 1)
                {
                    // Wait until the play event is fired
                    _play.WaitOne();
                }
                else
                {
                    Console.WriteLine("SCAN");
                    networks = GetNetworksAndScan();
                    foreach (var network in networks)
                    {
                        Console.WriteLine("Network SSID: {0} SignalQuality: {1}",
                                                  network.Ssid, network.SignalQuality);
                    }

                }
            }

            Console.WriteLine("Thread was stopped");
        }


        private IList<IMeasure> GetNetworksAndScan()
        {
            var measures = new List<IMeasure>();

            // get all wlan intrerfaces
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // only if interface is up (dormant???)
                if (wlanIface.NetworkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // Add the event to track when the wireless connection changes
                    //wlanIface.WlanNotification +=
                    //new WlanClient.WlanInterface.WlanNotificationEventHandler(Target);

                    // List all networks
                    Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0); //0 is a flag
                    ISet<string> alreadyListedSsids = new HashSet<string>();
                    foreach (Wlan.WlanAvailableNetwork network in networks)
                    {
                        // Trim ad-hoc networks
                        if (network.dot11BssType != Wlan.Dot11BssType.Independent)
                        {
                            string ssid = GetStringForSsid(network.dot11Ssid);
                            if (!alreadyListedSsids.Contains(ssid))
                            {
                                alreadyListedSsids.Add(ssid);
                                measures.Add(new SimpleMeasure(ssid, network.wlanSignalQuality));
                            }
                        }
                    }
                    // Ask for a new scan
                    wlanIface.Scan();
                }
            }
            return measures;
        }



        //private static void Target(Wlan.WlanNotificationData notifyData)
        //{
        //    if ((int)notifyData.NotificationCode == (int)Wlan.WlanNotificationCodeAcm.ScanComplete)
        //    {
        //        Console.WriteLine((int)notifyData.NotificationCode);
        //        foreach (var wlanInt in client.Interfaces)
        //        {
        //            if (wlanInt.InterfaceGuid == (Guid)notifyData.interfaceGuid)
        //            {
        //                //wlanInt
        //                //if(wlanInt.NetworkInterface.OperationalStatus == OperationalStatus.Dormant)
        //            }
        //        }
        //    }
        //    if ((int)notifyData.NotificationCode == (int)Wlan.WlanNotificationCodeAcm.ScanFail)
        //    {
        //        Console.WriteLine((int)notifyData.NotificationCode);
        //    }
        //}
    }
}