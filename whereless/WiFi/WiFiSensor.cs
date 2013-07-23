using System.Collections.Generic;
using System.Net.NetworkInformation;
using NativeWifi;
using System;
using System.Text;

namespace whereless.WiFi
{
    public class WiFiSensor
    {
        private WlanClient client;
        private System.Threading.ManualResetEvent _continueThreadEvent;
        public volatile bool StopThread = false;

        public WiFiSensor(System.Threading.ManualResetEvent continueThread)
        {
            client = new WlanClient();
            this._continueThreadEvent = continueThread;
        }

        // Converts a 802.11 SSID to a string.
        static string GetStringForSsid(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        public void ExampleMain()
        {
            for (int i = 0; !StopThread && i < 10; i++)
            {
                _continueThreadEvent.WaitOne();

                Console.WriteLine("SCAN " + i);
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
                                    Console.WriteLine("Network SSID: {0} SignalQuality: {1}",
                                                ssid, network.wlanSignalQuality);    
                                }
                                
                            }
                        }

                        // Ask for a new scan
                        wlanIface.Scan();
                    }

                    // A windows logo compliant wlan interface needs to complete the scan in at most 4 seconds
                    // 300ms given for thread spawning (just to be sure)
                    System.Threading.Thread.Sleep(4300);
                }
            }
            
            if (StopThread)
            {
                Console.WriteLine("Thread was stopped");
            }
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