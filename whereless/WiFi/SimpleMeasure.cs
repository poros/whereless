using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whereless.WiFi
{
    class SimpleMeasure : IMeasure
    {
        public string Ssid { get; set; }
        public uint SignalQuality { get; set; }

        public SimpleMeasure(string ssid, uint signalQuality)
        {
            this.Ssid = ssid;
            this.SignalQuality = signalQuality;
        }
    }
}
