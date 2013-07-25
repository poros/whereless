using log4net;
using NativeWifi;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using whereless.Model.ValueObjects;

namespace whereless.Controller.WiFi
{
    public class WiFiSensor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WiFiSensor));

        // A windows logo compliant wlan interface needs to complete the scan in at most 4 seconds
        // 300ms given for thread spawning (just to be sure)
        public const int ScanTime = 4300;

        private readonly WaitHandle[] _threadControls = new WaitHandle[2];
        private readonly WaitHandle _play;
        private readonly SensorToLocalizer<SensorOutput> _output; 
        private readonly WlanClient _client;


        // TODO Just a stub to remember to implement a delegate
        public delegate void UiDelegate();
        public UiDelegate WlanInterfaceDownDelegate { get; set; }


        // Converts a 802.11 SSID to a string.
        private static string GetStringForSsid(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }


        public WiFiSensor(WaitHandle stopThread, WaitHandle pauseThread, WaitHandle playThread, SensorToLocalizer<SensorOutput> output)
        {
            _client = new WlanClient();
            _threadControls[0] = stopThread;
            _threadControls[1] = pauseThread;
            _play = playThread;
            _output = output;
        }

        public void WiFiSensorLoop()
        {
            //First scan performed without waiting
            PerformScan();

            while (true)
            {
                int handle = WaitHandle.WaitAny(_threadControls, ScanTime);
                if (handle == 0)
                {
                    break;

                }
                else if (handle == 1)
                {
                    Log.Debug("WiFiSensor thread paused");
                    // Wait until the play event is fired
                    _play.WaitOne();
                    Log.Debug("WiFiSensor thread played");
                }
                else
                {
                    PerformScan();
                }
            }
            _output.Close();
            Log.Debug("WiFISensor thread has been stopped");
        }


        private void PerformScan()
        {
            Log.Debug("SCAN");
            var networks = GetNetworksAndScan();
            foreach (var network in networks)
            {
                Log.Debug("Network SSID: " + network.Ssid + " SignalQuality: " + network.SignalQuality);
            }
            //Remove (if present) previous SensorOutput and substitute it with the new one
            _output.LossyPut(new SensorOutput() {Measures = networks});
        }


        private IList<IMeasure> GetNetworksAndScan()
        {
            IList<IMeasure> measures = new List<IMeasure>();

            // get all wlan intrerfaces
            foreach (WlanClient.WlanInterface wlanIface in _client.Interfaces)
            {
                // Scan only if interface is up (dormant???)
                if (wlanIface.NetworkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    if (WlanInterfaceDownDelegate != null)
                    {
                        // TODO Just a stub to remember to implement a delegate
                        WlanInterfaceDownDelegate.BeginInvoke(null, this);
                        return null;
                    }
                }
                else
                {
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

                    // Alternative to timeout (abandoned idea)
                    // Add the event to track when the wireless connection changes
                    //wlanIface.WlanNotification +=
                    //new WlanClient.WlanInterface.WlanNotificationEventHandler(Target);

                    // Ask for a new scan
                    wlanIface.Scan();
                }
            }
            return measures;
        }


        // Notification for completed scan (abandoned idea)
        //private static void Target(Wlan.WlanNotificationData notifyData)
        //{
        //    if ((int)notifyData.NotificationCode == (int)Wlan.WlanNotificationCodeAcm.ScanComplete)
        //    {
        //        Log.Debug((int)notifyData.NotificationCode);
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
        //        Log.Debug((int)notifyData.NotificationCode);
        //    }
        //}
    }
}