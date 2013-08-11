using System;
using whereless.Model.ValueObjects;

namespace whereless.Model.Entities
{
    // a normal distribution of the power of a network over time is assumed
    public class GaussianNetwork : Network
    {
        // range of acceptance (corresponds to 99.99%)
        public static readonly double K = 1.96D;
        // signal quality varies between 0 and 100
        public static readonly double SignalQualityMin = 0D;
        public static readonly double SignalQualityMax = 100D;
        public static readonly double SignalQualityUnit = 1D;
        // number of observations to consider the distribution stable
        public static readonly ulong StableN = 10U;

        private static void CheckInput(IMeasure measure)
        {
            if (measure == null)
            {
                throw new ArgumentNullException("measure");
            }

            if (measure.SignalQuality < SignalQualityMin || measure.SignalQuality > SignalQualityMax)
            {
                throw new ArgumentOutOfRangeException("measure" + " SignalQuality out of range");
            }
        }

        private ulong _n;
        private double _mean;
        private double _s;


        public virtual ulong N
        {
            get { return _n; }
            set { _n = value; }
        }

        public virtual double Mean
        {
            get { return _mean; }
            set { _mean = value; }
        }

        public virtual double S
        {
            get { return _s; }
            set { _s = value; }

        }

        // REMARK Do not map in the db!!! S is enough and two of them may create inconsistencies
        public virtual double StdDev
        {
            get { return Math.Sqrt(_s / _n); }
            set { _s = value * value * _n; }
        }

        protected GaussianNetwork() { }

        public GaussianNetwork(IMeasure measure)
            : base(measure)
        {
            CheckInput(measure);
            _mean = measure.SignalQuality;
            _s = 0D;
            _n = 1;
        }


        public override bool TestInput(IMeasure measure)
        {
            CheckInput(measure);

            // SSIDs have not a standard character set 
            if (Ssid.Equals(measure.Ssid, StringComparison.Ordinal))
            {
                // if not enough stable (and if N = 1 then stdDev = 0)
                if (N < StableN)
                {
                    return true;
                }

                double dist = Math.Abs((measure.SignalQuality - Mean));
                double range = K * StdDev;
                
                // if stdDev < misure error (input is uint)
                if (range.CompareTo(SignalQualityUnit) > 0)
                {
                    return (dist.CompareTo(range) <= 0); //double safe comparison    
                }
                else
                {
                    return (dist.CompareTo(SignalQualityUnit) <= 0);
                }

            }
            return false;
        }

        public override void UpdateStats(IMeasure measure)
        {
            CheckInput(measure);

            //online mean and standard deviation
            var sample = (double)measure.SignalQuality;
            N += 1;
            var delta = sample - Mean;
            Mean = Mean + (delta / N);
            _s = _s + delta * (sample - Mean);
            // if overflow consider _variance = _variance + delta/n * (sample - Mean)
            // but it may suffer by numeric cancellation
            if (_s.Equals(Double.PositiveInfinity))
            {
                throw new OverflowException("S overflowed!!!!");
            }
        }


        public override string ToString()
        {
            return (base.ToString() + " Mean = " + Mean + "; S = " + S + "; StdDev = " + StdDev +
                    "; N = " + N + ";");
        }
    }
}
