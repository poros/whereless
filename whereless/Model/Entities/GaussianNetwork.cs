using System;
using whereless.NativeWiFi;

namespace whereless.Model.Entities
{
    // a normal distribution of the power of a network over time is assumed
    public class GaussianNetwork : Network
    {
        // range of acceptance (corresponds to 99.99%)
        private static readonly double k = 1.96D;
        // signal quality varies between 0 and 100
        private static readonly double signalQualityMax = 100D;
        

        private double _mean;
        private double _stdDev;
        private ulong _n;


        public static double SignalQualityMax
        {
            get { return signalQualityMax; }
        }

        public virtual double Mean
        {
            get { return _mean; }
            set { _mean = value; }
        }

        public virtual double StdDev
        {
            get { return _stdDev; }
            set { _stdDev = value; }
        }

        public virtual ulong N
        {
            get { return _n; }
            set { _n = value; }
        }


        protected GaussianNetwork() { }

        public GaussianNetwork(IMeasure measure) : base(measure)
        {
            _mean = measure.SignalQuality;
            _stdDev = 0D;
            _n = 1;
        }


        public override bool TestInput(IMeasure measure)
        {
            // SSIDs have not a standard character set 
            if (Ssid.Equals(measure.Ssid, StringComparison.Ordinal))
            {
                double dist = Math.Abs((measure.SignalQuality - Mean));
                return (dist.CompareTo(k*StdDev) <= 0); //double safe comparison
            }
            return false;
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


        public override string ToString()
        {
            return (base.ToString() + " Mean = " + Mean + "; StdDev = " + StdDev +
                    "; N = " + N + ";");
        }
    }
}
