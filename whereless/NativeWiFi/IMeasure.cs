using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whereless.NativeWiFi
{
    public interface IMeasure
    {
        string Ssid { get; set; }
        uint SignalQuality { get; set; }
    }
}
