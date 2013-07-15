using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    //a normal distribution of the power of a network over time is assumed
    public class GaussianNetwork : Network
    {
        public virtual double Mean { get; set; }
        public virtual double StdDev { get; set; }
        public virtual ulong N { get; set; }

        private static readonly double signalQualityMax = 100D;
        public static double SignalQualityMax { get { return signalQualityMax; } }

        protected GaussianNetwork() { }

        public GaussianNetwork(IMeasure measure)
        {

            Ssid = measure.Ssid;
            Mean = (double) measure.SignalQuality;
            StdDev = 0D;
            N = 1;
        }

        public override void UpdateStats(IMeasure measure)
        {
            //online mean and standard deviation
            var sample = (double) measure.SignalQuality;
            N += 1;
            var delta = sample - Mean;
            Mean = Mean + (delta / N);
            StdDev = StdDev + delta * (sample - Mean);
        }

        //public virtual Place PlaceReference { get; set; }
    }
}
